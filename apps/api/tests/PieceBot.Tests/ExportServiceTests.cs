using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Exports;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class ExportServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string Month = "2026-06";

    // Le service rend désormais un vrai fichier (QuestPDF / ClosedXML) et le stocke :
    // chaque test exerce donc la génération réelle de bout en bout.
    private static ExportService NewService() =>
        new(
            new InMemoryExportJobRepository(),
            new InMemoryPieceRepository(),
            new InMemoryEndClientRepository(),
            new PdfExcelExportRenderer(),
            new InMemoryExportFileStore());

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
    public async Task CreateMonthlyExportAsync_Pdf_GeneratesRealPdfBytes()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);
        var file = await service.GetFileAsync(TenantId, job.Id);

        Assert.NotNull(file);
        Assert.True(file!.Bytes.Length > 1000);
        Assert.Equal("application/pdf", file.ContentType);
        // En-tête magique PDF : "%PDF".
        Assert.Equal(new byte[] { 0x25, 0x50, 0x44, 0x46 }, file.Bytes.Take(4).ToArray());
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_Excel_GeneratesRealXlsxBytes()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Excel, Month);
        var file = await service.GetFileAsync(TenantId, job.Id);

        Assert.NotNull(file);
        Assert.True(file!.Bytes.Length > 1000);
        // Un .xlsx est une archive ZIP : signature "PK".
        Assert.Equal(new byte[] { 0x50, 0x4B }, file.Bytes.Take(2).ToArray());
    }

    [Fact]
    public async Task CreateMonthlyExportAsync_BatchExport_CountsAllMonthPieces()
    {
        var service = NewService();

        var job = await service.CreateMonthlyExportAsync(TenantId, ExportFormat.Pdf, Month);

        // 6 pièces de démo sur 2026-06.
        Assert.Equal(6, job.PieceCount);
        // Export par lot → nom de fichier « cabinet ».
        Assert.Contains("cabinet", job.FileName);
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
        // Nom de fichier basé sur le mois (+ slug du nom d'entreprise).
        Assert.StartsWith("piecebot_2026-06_", job.FileName);
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
