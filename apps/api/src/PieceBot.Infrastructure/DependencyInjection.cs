using Microsoft.Extensions.DependencyInjection;
using PieceBot.Core.Abstractions;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Infrastructure;

/// <summary>
/// Point d'entrée d'enregistrement de la couche Infrastructure dans le conteneur
/// DI. Aujourd'hui en mémoire ; ici qu'on branchera Cosmos DB, Blob Storage,
/// Document Intelligence, etc.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Singleton pour conserver l'état entre requêtes tant que c'est en mémoire.
        services.AddSingleton<IEndClientRepository, InMemoryEndClientRepository>();
        services.AddSingleton<IPieceRepository, InMemoryPieceRepository>();
        services.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        return services;
    }
}
