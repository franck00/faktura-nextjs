using System.Net;
using Microsoft.Azure.Cosmos;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Cosmos;

/// <summary>Repository Cosmos DB des pièces (spec §4.3). Partition <c>tenantId</c>.</summary>
public sealed class CosmosPieceRepository : IPieceRepository
{
    private readonly Container _container;

    public CosmosPieceRepository(CosmosClient client)
    {
        _container = client.GetContainer(CosmosNames.Database, CosmosNames.Pieces);
    }

    public async Task<IReadOnlyList<Piece>> ListAsync(
        string tenantId,
        string? month = null,
        string? endClientId = null,
        PieceStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM c WHERE c.type = 'piece'";
        if (month is not null) sql += " AND c.month = @month";
        if (endClientId is not null) sql += " AND c.endClientId = @endClientId";
        if (status is not null) sql += " AND c.status = @status";

        var query = new QueryDefinition(sql);
        if (month is not null) query.WithParameter("@month", month);
        if (endClientId is not null) query.WithParameter("@endClientId", endClientId);
        // L'enum est stocké en snake_case (contrat) → on sérialise le membre.
        if (status is not null)
        {
            query.WithParameter("@status", ToSnake(status.Value.ToString()));
        }

        var results = new List<Piece>();
        using var iterator = _container.GetItemQueryIterator<Piece>(
            query,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(tenantId) });

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync(cancellationToken))
            {
                results.Add(item);
            }
        }

        return results
            .OrderByDescending(p => p.ReceivedAt)
            .ToList();
    }

    public async Task<Piece?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<Piece>(
                id, new PartitionKey(tenantId), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Piece> CreateAsync(Piece piece, CancellationToken cancellationToken = default)
    {
        var response = await _container.UpsertItemAsync(
            piece, new PartitionKey(piece.TenantId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<Piece?> UpdateAsync(Piece piece, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(piece.TenantId, piece.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var response = await _container.UpsertItemAsync(
            piece, new PartitionKey(piece.TenantId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _container.DeleteItemAsync<Piece>(
                id, new PartitionKey(tenantId), cancellationToken: cancellationToken);
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>PascalCase → snake_case (aligné sur JsonNamingPolicy.SnakeCaseLower).</summary>
    private static string ToSnake(string value) =>
        string.Concat(value.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
}
