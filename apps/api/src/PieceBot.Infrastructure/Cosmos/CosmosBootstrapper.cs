using Microsoft.Azure.Cosmos;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Cosmos;

/// <summary>
/// Provisionne la base et les conteneurs Cosmos au démarrage (idempotent), et
/// seed les données de démo si la base est vide. Débit partagé 1000 RU/s au
/// niveau base → couvert par le palier gratuit Cosmos.
/// </summary>
public sealed class CosmosBootstrapper
{
    private readonly CosmosClient _client;

    public CosmosBootstrapper(CosmosClient client)
    {
        _client = client;
    }

    public async Task EnsureProvisionedAsync(CancellationToken cancellationToken = default)
    {
        // Débit partagé au niveau base (1000 RU/s = palier gratuit).
        var dbResponse = await _client.CreateDatabaseIfNotExistsAsync(
            CosmosNames.Database,
            throughput: 1000,
            cancellationToken: cancellationToken);
        var database = dbResponse.Database;

        foreach (var container in new[] { CosmosNames.Tenants, CosmosNames.EndClients, CosmosNames.Pieces })
        {
            await database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(container, CosmosNames.PartitionKeyPath),
                cancellationToken: cancellationToken);
        }

        await SeedIfEmptyAsync(database, cancellationToken);
    }

    /// <summary>Seed les données de démo si le conteneur des clients est vide.</summary>
    private static async Task SeedIfEmptyAsync(Database database, CancellationToken cancellationToken)
    {
        var clients = database.GetContainer(CosmosNames.EndClients);

        using var iterator = clients.GetItemQueryIterator<int>(
            new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));
        var existing = 0;
        if (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            existing = page.FirstOrDefault();
        }

        if (existing > 0)
        {
            return;
        }

        var tenants = database.GetContainer(CosmosNames.Tenants);
        var pieces = database.GetContainer(CosmosNames.Pieces);

        var tenant = DemoData.Tenant();
        await tenants.UpsertItemAsync(tenant, new PartitionKey(tenant.TenantId), cancellationToken: cancellationToken);

        foreach (var client in DemoData.EndClients())
        {
            await clients.UpsertItemAsync(client, new PartitionKey(client.TenantId), cancellationToken: cancellationToken);
        }

        foreach (var piece in DemoData.Pieces())
        {
            await pieces.UpsertItemAsync(piece, new PartitionKey(piece.TenantId), cancellationToken: cancellationToken);
        }
    }
}
