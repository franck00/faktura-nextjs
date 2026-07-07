using PieceBot.Core.Abstractions;

namespace PieceBot.Infrastructure.Billing;

/// <summary>
/// Implémentation provisoire de <see cref="IStripeGateway"/> : ne crée pas de
/// vraie session Stripe, renvoie une URL déterministe. À remplacer par le SDK
/// Stripe (Stripe.net) + clé secrète.
/// </summary>
public sealed class StubStripeGateway : IStripeGateway
{
    public Task<string> CreateBillingPortalSessionAsync(
        string stripeCustomerId,
        string returnUrl,
        CancellationToken cancellationToken = default)
    {
        var url = $"https://billing.stripe.com/p/session/{stripeCustomerId}";
        return Task.FromResult(url);
    }
}
