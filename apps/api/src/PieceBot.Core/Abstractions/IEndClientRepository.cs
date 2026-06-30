using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Accès aux clients d'un cabinet (spec §4.2 / §6.3). L'implémentation Cosmos
/// vivra dans PieceBot.Infrastructure ; les requêtes sont toujours scopées par
/// <paramref name="tenantId"/> (isolation multi-tenant stricte).
/// </summary>
public interface IEndClientRepository
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
