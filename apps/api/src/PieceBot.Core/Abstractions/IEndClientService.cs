using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL pour les opérations EndClients. L'API consomme ce service,
/// qui orchestre les règles métier et délègue la persistance au repository DAL.
/// </summary>
public interface IEndClientService
{
    Task<IReadOnlyList<EndClient>> ListAsync(
        string tenantId,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<EndClient?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);

    Task<EndClient> CreateAsync(EndClient client, CancellationToken cancellationToken = default);

    Task<EndClient?> UpdateAsync(EndClient client, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée en masse des clients depuis un contenu CSV (spec §6.3). Colonnes
    /// attendues : <c>companyName, contactName, whatsappNumber, email, siret,
    /// vatNumber, tags, activeMonth</c> (les 3 premières sont requises ; <c>tags</c>
    /// séparés par <c>;</c>). Les lignes invalides sont rapportées sans bloquer le reste.
    /// </summary>
    Task<ImportCsvResult> ImportCsvAsync(
        string tenantId,
        string csvContent,
        CancellationToken cancellationToken = default);
}
