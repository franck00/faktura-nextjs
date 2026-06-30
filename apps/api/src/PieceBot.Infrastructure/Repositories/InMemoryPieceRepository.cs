using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Repositories;

/// <summary>
/// Implémentation mémoire provisoire de <see cref="IPieceRepository"/>.
/// Seedée avec des données de démo (miroir du frontend). À remplacer par un
/// repository Cosmos DB (partition tenantId) sans changer l'interface.
/// </summary>
public sealed class InMemoryPieceRepository : IPieceRepository
{
    private readonly ConcurrentDictionary<string, Piece> _pieces = new();

    public InMemoryPieceRepository()
    {
        foreach (var piece in DemoData.Pieces())
        {
            _pieces[piece.Id] = piece;
        }
    }

    public Task<IReadOnlyList<Piece>> ListAsync(
        string tenantId,
        string? month = null,
        string? endClientId = null,
        PieceStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Piece> query = _pieces.Values.Where(p => p.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(month))
        {
            query = query.Where(p => p.Month == month);
        }

        if (!string.IsNullOrWhiteSpace(endClientId))
        {
            query = query.Where(p => p.EndClientId == endClientId);
        }

        if (status is not null)
        {
            query = query.Where(p => p.Status == status);
        }

        IReadOnlyList<Piece> result = query
            .OrderByDescending(p => p.ReceivedAt)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<Piece?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        _pieces.TryGetValue(id, out var piece);
        return Task.FromResult(piece is not null && piece.TenantId == tenantId ? piece : null);
    }

    public Task<Piece> CreateAsync(Piece piece, CancellationToken cancellationToken = default)
    {
        _pieces[piece.Id] = piece;
        return Task.FromResult(piece);
    }

    public Task<Piece?> UpdateAsync(Piece piece, CancellationToken cancellationToken = default)
    {
        if (!_pieces.ContainsKey(piece.Id))
        {
            return Task.FromResult<Piece?>(null);
        }

        _pieces[piece.Id] = piece;
        return Task.FromResult<Piece?>(piece);
    }

    public Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        if (_pieces.TryGetValue(id, out var piece) && piece.TenantId == tenantId)
        {
            return Task.FromResult(_pieces.TryRemove(id, out _));
        }

        return Task.FromResult(false);
    }
}
