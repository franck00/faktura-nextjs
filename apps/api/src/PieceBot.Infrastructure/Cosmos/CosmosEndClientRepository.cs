using System.Net;
using Microsoft.Azure.Cosmos;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Cosmos;

/// <summary>Repository Cosmos DB des clients du cabinet (spec §4.2). Partition <c>tenantId</c>.</summary>
public sealed class CosmosEndClientRepository : IEndClientRepository
{
    private readonly Container _container;

    public CosmosEndClientRepository(CosmosClient client)
    {
        _container = client.GetContainer(CosmosNames.Database, CosmosNames.EndClients);
    }

    public async Task<IReadOnlyList<EndClient>> ListAsync(
        string tenantId,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.type = 'endclient'");
        var results = new List<EndClient>();
        using var iterator = _container.GetItemQueryIterator<EndClient>(
            query,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(tenantId) });

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync(cancellationToken))
            {
                results.Add(item);
            }
        }

        IEnumerable<EndClient> filtered = results;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            filtered = results.Where(c =>
                c.CompanyName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.ContactName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.WhatsappNumber.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.Tags.Any(t => t.Contains(q, StringComparison.OrdinalIgnoreCase)));
        }

        return filtered
            .OrderBy(c => c.CompanyName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<EndClient?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<EndClient>(
                id, new PartitionKey(tenantId), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<EndClient> CreateAsync(EndClient client, CancellationToken cancellationToken = default)
    {
        var response = await _container.UpsertItemAsync(
            client, new PartitionKey(client.TenantId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<EndClient?> UpdateAsync(EndClient client, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(client.TenantId, client.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var response = await _container.UpsertItemAsync(
            client, new PartitionKey(client.TenantId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _container.DeleteItemAsync<EndClient>(
                id, new PartitionKey(tenantId), cancellationToken: cancellationToken);
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
