using System.Net;
using Microsoft.Azure.Cosmos;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Cosmos;

/// <summary>Repository Cosmos DB du cabinet (spec §4.1). Partition <c>tenantId</c>.</summary>
public sealed class CosmosTenantRepository : ITenantRepository
{
    private readonly Container _container;

    public CosmosTenantRepository(CosmosClient client)
    {
        _container = client.GetContainer(CosmosNames.Database, CosmosNames.Tenants);
    }

    public async Task<Tenant?> GetAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<Tenant>(
                tenantId, new PartitionKey(tenantId), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Tenant?> GetByWhatsappPhoneNumberIdAsync(
        string phoneNumberId,
        CancellationToken cancellationToken = default)
    {
        // Requête inter-partitions (le tenant n'est pas connu à ce stade).
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'tenant' AND c.whatsappPhoneNumberId = @pid")
            .WithParameter("@pid", phoneNumberId);

        using var iterator = _container.GetItemQueryIterator<Tenant>(query);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            var match = page.FirstOrDefault();
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    public async Task<Tenant?> GetByStripeCustomerIdAsync(
        string stripeCustomerId,
        CancellationToken cancellationToken = default)
    {
        // Requête inter-partitions (le tenant n'est pas connu à ce stade).
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'tenant' AND c.stripeCustomerId = @cid")
            .WithParameter("@cid", stripeCustomerId);

        using var iterator = _container.GetItemQueryIterator<Tenant>(query);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            var match = page.FirstOrDefault();
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    public async Task<Tenant?> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(tenant.TenantId, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var response = await _container.UpsertItemAsync(
            tenant, new PartitionKey(tenant.TenantId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _container.UpsertItemAsync(
            tenant, new PartitionKey(tenant.TenantId), cancellationToken: cancellationToken);
    }
}
