using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Accès au cabinet courant (collection Cosmos <c>tenants</c>, spec §4.1 / §6.2).
/// <see cref="GetByWhatsappPhoneNumberIdAsync"/> servira au webhook pour résoudre
/// le tenant à partir du Phone Number ID Meta (spec §3.1).
/// </summary>
public interface ITenantRepository
{
    Task<Tenant?> GetAsync(string tenantId, CancellationToken cancellationToken = default);

    Task<Tenant?> GetByWhatsappPhoneNumberIdAsync(
        string phoneNumberId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Résout le cabinet à partir de son identifiant client Stripe (spec §5.8).
    /// Sert au webhook Stripe pour appliquer le cycle de vie de l'abonnement.
    /// </summary>
    Task<Tenant?> GetByStripeCustomerIdAsync(
        string stripeCustomerId,
        CancellationToken cancellationToken = default);

    Task<Tenant?> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
