using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class EndClientServiceTests
{
    private const string TenantId = "tenant_mvogo";

    [Fact]
    public async Task ListAsync_TrimsSearchTerm()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());

        var clients = await service.ListAsync(TenantId, "  batipro ");

        Assert.Single(clients);
        Assert.Contains("BatiPro", clients[0].CompanyName);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCompanyNameMissing()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());

        var invalid = new EndClient
        {
            Id = "client_new",
            TenantId = TenantId,
            CompanyName = "",
            ContactName = "Test Contact",
            WhatsappNumber = "+237600000000",
            ActiveMonth = "2026-06",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(invalid));
    }
}
