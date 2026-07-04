using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL de facturation Stripe (spec §5.8). Gère la vérification de
/// signature des webhooks, l'application du cycle de vie de l'abonnement au
/// tenant, et l'ouverture du portail client Stripe.
/// </summary>
public interface IBillingService
{
    /// <summary>
    /// Valide l'en-tête <c>Stripe-Signature</c> (schéma <c>t=…,v1=…</c>, HMAC
    /// SHA-256 de <c>{timestamp}.{corps}</c>).
    /// </summary>
    bool VerifySignature(byte[] rawBody, string? signatureHeader, string webhookSecret);

    /// <summary>Applique un évènement Stripe au cabinet concerné.</summary>
    Task<BillingWebhookResult> HandleEventAsync(
        StripeEvent stripeEvent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une URL de portail client Stripe pour le tenant, ou <c>null</c> s'il
    /// n'a pas encore de client Stripe rattaché.
    /// </summary>
    Task<string?> CreatePortalUrlAsync(
        string tenantId,
        string returnUrl,
        CancellationToken cancellationToken = default);
}
