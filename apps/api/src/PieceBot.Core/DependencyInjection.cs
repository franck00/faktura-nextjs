using Microsoft.Extensions.DependencyInjection;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Services;

namespace PieceBot.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IEndClientService, EndClientService>();
        services.AddScoped<IPieceService, PieceService>();
        services.AddScoped<IStatsService, StatsService>();
        return services;
    }
}
