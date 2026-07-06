using System.Security.Cryptography;
using System.Text;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>
/// Implémentation BLL de la facturation Stripe (spec §5.8). Vérifie les
/// signatures de webhook, traduit les évènements en cycle de vie d'abonnement
/// sur le tenant, et ouvre le portail client via <see cref="IStripeGateway"/>.
/// </summary>
public sealed class BillingService : IBillingService
{
    /// <summary>Tolérance d'horodatage du webhook Stripe (anti-rejeu).</summary>
    private static readonly TimeSpan SignatureTolerance = TimeSpan.FromMinutes(5);

    private readonly ITenantRepository _tenants;
    private readonly IStripeGateway _gateway;

    public BillingService(ITenantRepository tenants, IStripeGateway gateway)
    {
        _tenants = tenants;
        _gateway = gateway;
    }

    public bool VerifySignature(byte[] rawBody, string? signatureHeader, string webhookSecret)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(webhookSecret))
        {
            return false;
        }

        // En-tête Stripe : "t=1492774577,v1=hex,v0=...".
        string? timestamp = null;
        var signatures = new List<string>();
        foreach (var part in signatureHeader.Split(','))
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2)
            {
                continue;
            }

            switch (kv[0].Trim())
            {
                case "t":
                    timestamp = kv[1].Trim();
                    break;
                case "v1":
                    signatures.Add(kv[1].Trim());
                    break;
            }
        }

        if (timestamp is null || signatures.Count == 0
            || !long.TryParse(timestamp, out var unix))
        {
            return false;
        }

        // Anti-rejeu : rejeter les horodatages hors tolérance.
        var age = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(unix);
        if (age.Duration() > SignatureTolerance)
        {
            return false;
        }

        var signedPayload = new byte[timestamp.Length + 1 + rawBody.Length];
        var tsBytes = Encoding.ASCII.GetBytes(timestamp);
        tsBytes.CopyTo(signedPayload, 0);
        signedPayload[tsBytes.Length] = (byte)'.';
        rawBody.CopyTo(signedPayload, tsBytes.Length + 1);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
        var expected = Convert.ToHexStringLower(hmac.ComputeHash(signedPayload));
        var expectedBytes = Encoding.ASCII.GetBytes(expected);

        return signatures.Any(sig =>
            CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(sig), expectedBytes));
    }

    public async Task<BillingWebhookResult> HandleEventAsync(
        StripeEvent stripeEvent,
        CancellationToken cancellationToken = default)
    {
        var newStatus = MapStatus(stripeEvent);
        if (newStatus is null || string.IsNullOrEmpty(stripeEvent.CustomerId))
        {
            return BillingWebhookResult.Ignored();
        }

        var tenant = await _tenants.GetByStripeCustomerIdAsync(stripeEvent.CustomerId, cancellationToken);
        if (tenant is null)
        {
            return BillingWebhookResult.TenantNotFound();
        }

        tenant.SubscriptionStatus = newStatus.Value;
        await _tenants.UpdateAsync(tenant, cancellationToken);

        return new BillingWebhookResult
        {
            Outcome = BillingWebhookOutcome.SubscriptionUpdated,
            TenantId = tenant.TenantId,
            NewStatus = newStatus.Value
        };
    }

    public async Task<string?> CreatePortalUrlAsync(
        string tenantId,
        string returnUrl,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _tenants.GetAsync(tenantId, cancellationToken);
        if (tenant?.StripeCustomerId is null)
        {
            return null;
        }

        return await _gateway.CreateBillingPortalSessionAsync(
            tenant.StripeCustomerId, returnUrl, cancellationToken);
    }

    /// <summary>Traduit un évènement Stripe en statut d'abonnement, ou <c>null</c> si non pertinent.</summary>
    private static SubscriptionStatus? MapStatus(StripeEvent stripeEvent) => stripeEvent.Type switch
    {
        "checkout.session.completed" => SubscriptionStatus.Active,
        "invoice.payment_failed" => SubscriptionStatus.PastDue,
        "customer.subscription.deleted" => SubscriptionStatus.Canceled,
        "customer.subscription.created" or "customer.subscription.updated" =>
            MapSubscriptionStatus(stripeEvent.SubscriptionStatus),
        _ => null
    };

    private static SubscriptionStatus? MapSubscriptionStatus(string? raw) => raw switch
    {
        "active" => SubscriptionStatus.Active,
        "trialing" => SubscriptionStatus.Trialing,
        "past_due" or "unpaid" => SubscriptionStatus.PastDue,
        "canceled" or "incomplete_expired" => SubscriptionStatus.Canceled,
        _ => null
    };
}
