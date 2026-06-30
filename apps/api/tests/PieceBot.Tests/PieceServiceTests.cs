using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class PieceServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string Month = "2026-06";

    [Fact]
    public async Task ListAsync_FiltersByStatus()
    {
        var service = new PieceService(new InMemoryPieceRepository());

        var validated = await service.ListAsync(TenantId, Month, status: PieceStatus.Validated);

        Assert.NotEmpty(validated);
        Assert.All(validated, p => Assert.Equal(PieceStatus.Validated, p.Status));
    }

    [Fact]
    public async Task ListAsync_FiltersByEndClient()
    {
        var service = new PieceService(new InMemoryPieceRepository());

        var pieces = await service.ListAsync(TenantId, Month, endClientId: "client_batipro");

        Assert.NotEmpty(pieces);
        Assert.All(pieces, p => Assert.Equal("client_batipro", p.EndClientId));
    }

    [Fact]
    public async Task ValidateAsync_SetsValidatedStatusAndAudit()
    {
        var service = new PieceService(new InMemoryPieceRepository());

        var piece = await service.ValidateAsync(TenantId, "piece_001", "user_test");

        Assert.NotNull(piece);
        Assert.Equal(PieceStatus.Validated, piece!.Status);
        Assert.Equal("user_test", piece.ValidatedBy);
        Assert.NotNull(piece.ValidatedAt);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsNull_WhenPieceMissing()
    {
        var service = new PieceService(new InMemoryPieceRepository());

        var piece = await service.ValidateAsync(TenantId, "piece_unknown", "user_test");

        Assert.Null(piece);
    }

    [Fact]
    public async Task CorrectAsync_UpdatesCategoryAndExtraction()
    {
        var service = new PieceService(new InMemoryPieceRepository());
        var newData = new ExtractedData
        {
            Supplier = "Corrigé SARL",
            TotalAmountTtc = 1000,
            Currency = "XAF"
        };

        var piece = await service.CorrectAsync(
            TenantId, "piece_002", PieceCategory.InvoicePurchase, newData);

        Assert.NotNull(piece);
        Assert.Equal(PieceCategory.InvoicePurchase, piece!.Category);
        Assert.Equal("Corrigé SARL", piece.ExtractedData.Supplier);
    }

    [Fact]
    public async Task GetAsync_IsTenantScoped()
    {
        var service = new PieceService(new InMemoryPieceRepository());

        var fromOtherTenant = await service.GetAsync("tenant_other", "piece_001");

        Assert.Null(fromOtherTenant);
    }
}
