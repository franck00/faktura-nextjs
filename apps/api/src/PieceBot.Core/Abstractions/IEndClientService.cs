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
}
