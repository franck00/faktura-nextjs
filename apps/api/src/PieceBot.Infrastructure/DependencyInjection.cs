using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PieceBot.Core.Abstractions;
using PieceBot.Infrastructure.Billing;
using PieceBot.Infrastructure.Cosmos;
using PieceBot.Infrastructure.Exports;
using PieceBot.Infrastructure.Messaging;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Infrastructure;

/// <summary>
/// Enregistrement de la couche Infrastructure. Les repositories cœur (clients,
/// pièces, tenants) basculent sur Cosmos DB si <c>Cosmos:Endpoint</c> +
/// <c>Cosmos:Key</c> sont configurés, sinon In-Memory. Les autres services
/// (rappels, exports, ports WhatsApp/Stripe) sont enregistrés dans les deux cas.
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
            AddCosmosRepositories(services, endpoint, key);
        }
        else
        {
            AddInMemoryRepositories(services);
        }

        // Repositories sans implémentation Cosmos pour l'instant (toujours In-Memory).
        services.AddSingleton<IMonthlyReminderRepository, InMemoryMonthlyReminderRepository>();
        services.AddSingleton<IExportJobRepository, InMemoryExportJobRepository>();

        // Exports réels : rendu PDF/Excel + stockage local des fichiers générés.
        services.AddSingleton<IExportRenderer, PdfExcelExportRenderer>();
        services.AddSingleton<IExportFileStore, InMemoryExportFileStore>();

        // Ports externes (stubs en attendant Meta Cloud API / Blob Storage / Stripe.net).
        services.AddSingleton<IWhatsAppMediaStore, StubWhatsAppMediaStore>();
        services.AddSingleton<IWhatsAppSender, StubWhatsAppSender>();
        services.AddSingleton<IStripeGateway, StubStripeGateway>();

        return services;
    }

    private static void AddInMemoryRepositories(IServiceCollection services)
    {
        // Singleton pour conserver l'état entre requêtes tant que c'est en mémoire.
        services.AddSingleton<IEndClientRepository, InMemoryEndClientRepository>();
        services.AddSingleton<IPieceRepository, InMemoryPieceRepository>();
        services.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
    }

    private static void AddCosmosRepositories(IServiceCollection services, string endpoint, string key)
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
