using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL du cabinet courant (spec §6.2). L'API consomme ce service ;
/// il applique les règles métier (champs modifiables, validation) et délègue
/// la persistance au <see cref="ITenantRepository"/>.
/// </summary>
public interface ITenantService
{
    Task<Tenant?> GetAsync(string tenantId, CancellationToken cancellationToken = default);

    Task<Tenant?> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>Lie le numéro WhatsApp Business du cabinet (spec §5.1 / §6.2).</summary>
    Task<Tenant?> LinkWhatsappAsync(
        string tenantId,
        string phoneNumberId,
        string businessAccountId,
        CancellationToken cancellationToken = default);
}
