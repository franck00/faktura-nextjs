using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class StatsServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string Month = "2026-06";

    private static StatsService NewService() =>
        new(new InMemoryPieceRepository(), new InMemoryEndClientRepository());

    [Fact]
    public async Task GetDashboardAsync_ComputesTotals()
    {
        var service = NewService();

        var stats = await service.GetDashboardAsync(TenantId, Month);

        Assert.Equal(Month, stats.Month);
        Assert.Equal(6, stats.TotalPieces);
        Assert.Equal(2, stats.ValidatedPieces);
        Assert.Equal(4, stats.PendingValidation);
        Assert.Equal(3, stats.ActiveClients);
    }

    [Fact]
    public async Task GetDashboardAsync_EmptyMonth_YieldsZeroCompletion()
    {
        var service = NewService();

        var stats = await service.GetDashboardAsync(TenantId, "2099-01");

        Assert.Equal(0, stats.TotalPieces);
        Assert.Equal(0, stats.CompletionRate);
    }

    [Fact]
    public async Task GetCompletionAsync_ReturnsPerClientRates()
    {
        var service = NewService();

        var completions = await service.GetCompletionAsync(TenantId, Month);

        var pharma = completions.Single(c => c.EndClientId == "client_pharma");
        Assert.Equal(1, pharma.PiecesReceived);
        Assert.Equal(1, pharma.PiecesValidated);
        Assert.Equal(1d, pharma.CompletionRate);

        var transport = completions.Single(c => c.EndClientId == "client_transport");
        Assert.Equal(0, transport.PiecesReceived);
        Assert.Equal(0d, transport.CompletionRate);
    }
}
