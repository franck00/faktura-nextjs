using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class TenantServiceTests
{
    private const string TenantId = "tenant_mvogo";

    [Fact]
    public async Task GetAsync_ReturnsSeededTenant()
    {
        var service = new TenantService(new InMemoryTenantRepository());

        var tenant = await service.GetAsync(TenantId);

        Assert.NotNull(tenant);
        Assert.Equal("Cabinet Mvogo & Associés", tenant!.Name);
        Assert.Equal("XAF", tenant.Currency);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_ForUnknownTenant()
    {
        var service = new TenantService(new InMemoryTenantRepository());

        var tenant = await service.GetAsync("tenant_unknown");

        Assert.Null(tenant);
    }

    [Fact]
    public async Task UpdateAsync_PersistsEditableFields()
    {
        var service = new TenantService(new InMemoryTenantRepository());
        var tenant = await service.GetAsync(TenantId);
        tenant!.Name = "Cabinet Renommé";
        tenant.VatRate = 18m;

        var updated = await service.UpdateAsync(tenant);

        Assert.NotNull(updated);
        Assert.Equal("Cabinet Renommé", updated!.Name);
        Assert.Equal(18m, updated.VatRate);
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenNameMissing()
    {
        var service = new TenantService(new InMemoryTenantRepository());
        var tenant = await service.GetAsync(TenantId);
        tenant!.Name = "";

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(tenant));
    }

    [Fact]
    public async Task LinkWhatsappAsync_SetsNumbers()
    {
        var service = new TenantService(new InMemoryTenantRepository());

        var updated = await service.LinkWhatsappAsync(TenantId, "999888777", "111222333");

        Assert.NotNull(updated);
        Assert.Equal("999888777", updated!.WhatsappPhoneNumberId);
        Assert.Equal("111222333", updated.WhatsappBusinessAccountId);
    }

    [Fact]
    public async Task LinkWhatsappAsync_Throws_WhenPhoneMissing()
    {
        var service = new TenantService(new InMemoryTenantRepository());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.LinkWhatsappAsync(TenantId, "  ", "x"));
    }

    [Fact]
    public async Task LinkWhatsappAsync_ReturnsNull_ForUnknownTenant()
    {
        var service = new TenantService(new InMemoryTenantRepository());

        var updated = await service.LinkWhatsappAsync("tenant_unknown", "1", "2");

        Assert.Null(updated);
    }
}
