using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Accès aux pièces justificatives d'un cabinet (spec §4.3 / §6.4). Toujours
/// scopé par <paramref name="tenantId"/> (partition Cosmos / isolation tenant).
/// L'implémentation Cosmos vivra dans PieceBot.Infrastructure.
/// </summary>
public interface IPieceRepository
{
    Task<IReadOnlyList<Piece>> ListAsync(
        string tenantId,
        string? month = null,
        string? endClientId = null,
        PieceStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<Piece?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);

    Task<Piece> CreateAsync(Piece piece, CancellationToken cancellationToken = default);

    Task<Piece?> UpdateAsync(Piece piece, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);
}
