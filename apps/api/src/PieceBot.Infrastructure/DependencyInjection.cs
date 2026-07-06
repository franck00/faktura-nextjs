using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PieceBot.Core.Abstractions;
using PieceBot.Infrastructure.Cosmos;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Infrastructure;

/// <summary>
/// Enregistrement de la couche Infrastructure. Si <c>Cosmos:Endpoint</c> et
/// <c>Cosmos:Key</c> sont configurés → repositories Cosmos DB ; sinon → In-Memory
/// (démo). Aucune régression : sans config Cosmos, comportement inchangé.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var endpoint = configuration["Cosmos:Endpoint"];
        var key = configuration["Cosmos:Key"];

        if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(key))
        {
            AddCosmos(services, endpoint, key);
        }
        else
        {
            AddInMemory(services);
        }

        return services;
    }

    private static void AddInMemory(IServiceCollection services)
    {
        // Singleton pour conserver l'état entre requêtes tant que c'est en mémoire.
        services.AddSingleton<IEndClientRepository, InMemoryEndClientRepository>();
        services.AddSingleton<IPieceRepository, InMemoryPieceRepository>();
        services.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
    }

    private static void AddCosmos(IServiceCollection services, string endpoint, string key)
    {
        services.AddSingleton(_ => new CosmosClient(endpoint, key, new CosmosClientOptions
        {
            Serializer = new SystemTextJsonCosmosSerializer(),
            ApplicationName = "piecebot-api"
        }));

        services.AddSingleton<CosmosBootstrapper>();

        services.AddScoped<IEndClientRepository, CosmosEndClientRepository>();
        services.AddScoped<IPieceRepository, CosmosPieceRepository>();
        services.AddScoped<ITenantRepository, CosmosTenantRepository>();
    }
}
