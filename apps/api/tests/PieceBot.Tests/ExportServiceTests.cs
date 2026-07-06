using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class ExportServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string Month = "2026-06";

    private static ExportService NewService() =>
        new(new InMemoryExportJobRepository(), new InMemoryPieceRepository());

    [Fact]
    public async Task CreateMonthlyExportAsync_Pdf_CompletesWithBlobUrl()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);

        Assert.Equal(ExportStatus.Completed, job.Status);
        Assert.Equal(ExportFormat.Pdf, job.Format);
        Assert.NotNull(job.BlobUrl);
        Assert.EndsWith(".pdf", job.FileName);
        Assert.NotNull(job.CompletedAt);
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_Excel_UsesXlsxExtension()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Excel, Month);

        Assert.EndsWith(".xlsx", job.FileName);
        Assert.Contains("/exports/", job.BlobUrl);
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_BatchExport_CountsAllMonthPieces()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);

        // 6 pièces de démo sur 2026-06.
        Assert.Equal(6, job.PieceCount);
        Assert.Contains("_all.pdf", job.FileName);
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_PerClient_FiltersAndAggregatesTotals()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(
            TenantId, ExportFormat.Pdf, Month, "client_aissa");

        // client_aissa : 3 pièces (78500 + 24000 + 45000 = 147500 TTC).
        Assert.Equal(3, job.PieceCount);
        Assert.Equal(147500, job.TotalAmountTtc);
        Assert.Contains("client_aissa", job.FileName);
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_Throws_WhenMonthMissing()
    {
        var service = NewService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, ""));
    }

    [Fact]
    public async Task GetAsync_ReturnsCreatedJob()
    {
        var service = NewService();
        var created = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);

        var fetched = await service.GetAsync(TenantId, created.Id);

        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetAsync_IsTenantScoped()
    {
        var service = NewService();
        var created = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);

        var fromOtherTenant = await service.GetAsync("tenant_other", created.Id);

        Assert.Null(fromOtherTenant);
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_EmptyMonth_HasNoPieces()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Excel, "2099-01");

        Assert.Equal(ExportStatus.Completed, job.Status);
        Assert.Equal(0, job.PieceCount);
        Assert.Equal(0, job.TotalAmountTtc);
    }
}
