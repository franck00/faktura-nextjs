using PieceBot.Core.Domain;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class InMemoryEndClientRepositoryTests
{
    private const string TenantId = "tenant_mvogo";

    [Fact]
    public async Task ListAsync_ReturnsSeededClients_ForTenant()
    {
        var repo = new InMemoryEndClientRepository();

        var clients = await repo.ListAsync(TenantId);

        Assert.NotEmpty(clients);
        Assert.All(clients, c => Assert.Equal(TenantId, c.TenantId));
    }

    [Fact]
    public async Task ListAsync_FiltersBySearch()
    {
        var repo = new InMemoryEndClientRepository();

        var clients = await repo.ListAsync(TenantId, "batipro");

        Assert.Single(clients);
        Assert.Contains("BatiPro", clients[0].CompanyName);
    }

    [Fact]
    public async Task GetAsync_IsTenantScoped()
    {
        var repo = new InMemoryEndClientRepository();

        var fromOtherTenant = await repo.GetAsync("tenant_other", "client_aissa");

        Assert.Null(fromOtherTenant);
    }

    [Fact]
    public async Task DeleteAsync_RemovesClient()
    {
        var repo = new InMemoryEndClientRepository();

        var deleted = await repo.DeleteAsync(TenantId, "client_aissa");
        var afterDelete = await repo.GetAsync(TenantId, "client_aissa");

        Assert.True(deleted);
        Assert.Null(afterDelete);
    }
}
