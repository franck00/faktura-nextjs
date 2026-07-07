namespace PieceBot.Core.Abstractions;

/// <summary>
/// Port vers l'API Stripe (spec §5.8). L'implémentation réelle (Infrastructure)
/// utilisera le SDK Stripe ; un stub renvoie une URL déterministe en attendant.
/// </summary>
public interface IStripeGateway
{
    /// <summary>Crée une session du portail client Stripe et renvoie son URL.</summary>
    Task<string> CreateBillingPortalSessionAsync(
        string stripeCustomerId,
        string returnUrl,
        CancellationToken cancellationToken = default);
}
