using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>Implémentation BLL des exports mensuels (spec §5.7 / §6.5).</summary>
public sealed class ExportService : IExportService
{
    private readonly IExportJobRepository _jobs;
    private readonly IPieceRepository _pieces;
    private readonly IEndClientRepository _clients;
    private readonly IExportRenderer _renderer;
    private readonly IExportFileStore _store;

    public ExportService(
        IExportJobRepository jobs,
        IPieceRepository pieces,
        IEndClientRepository clients,
        IExportRenderer renderer,
        IExportFileStore store)
    {
        _jobs = jobs;
        _pieces = pieces;
        _clients = clients;
        _renderer = renderer;
        _store = store;
    }

    public async Task<ExportJob> CreateMonthlyExportAsync(
        string tenantId,
        ExportFormat format,
        string month,
        string? endClientId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            throw new ArgumentException("month requis (format YYYY-MM)", nameof(month));
        }

        var job = new ExportJob
        {
            Id = $"export_{Guid.NewGuid():N}",
            TenantId = tenantId,
            Format = format,
            Month = month,
            EndClientId = endClientId,
            Status = ExportStatus.Queued,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _jobs.CreateAsync(job, cancellationToken);

        try
        {
            job.Status = ExportStatus.Processing;
            await _jobs.UpdateAsync(job, cancellationToken);

            var pieces = await _pieces.ListAsync(
                tenantId, month, endClientId, cancellationToken: cancellationToken);

            job.PieceCount = pieces.Count;
            job.TotalAmountTtc = pieces.Sum(p => p.ExtractedData.TotalAmountTtc ?? 0);

            // Nom d'entreprise par client, pour l'affichage dans le fichier.
            var clients = await _clients.ListAsync(tenantId, null, cancellationToken);
            var clientNames = clients.ToDictionary(c => c.Id, c => c.CompanyName);

            // Rendu réel (QuestPDF pour PDF, ClosedXML pour Excel) puis stockage.
            var file = _renderer.Render(job, pieces, clientNames);
            await _store.SaveAsync(tenantId, job.Id, file, cancellationToken);

            job.FileName = file.FileName;
            // URL de téléchargement servie par l'API (Blob Storage + SAS plus tard).
            job.BlobUrl = $"/api/exports/{job.Id}/download";

            job.Status = ExportStatus.Completed;
            job.CompletedAt = DateTimeOffset.UtcNow;
            await _jobs.UpdateAsync(job, cancellationToken);
        }
        catch (Exception ex)
        {
            job.Status = ExportStatus.Failed;
            job.Error = ex.Message;
            job.CompletedAt = DateTimeOffset.UtcNow;
            await _jobs.UpdateAsync(job, cancellationToken);
        }

        return job;
    }

    public Task<ExportJob?> GetAsync(
        string tenantId,
        string jobId,
        CancellationToken cancellationToken = default)
    {
        return _jobs.GetAsync(tenantId, jobId, cancellationToken);
    }

    public Task<ExportFile?> GetFileAsync(
        string tenantId,
        string jobId,
        CancellationToken cancellationToken = default)
    {
        return _store.OpenAsync(tenantId, jobId, cancellationToken);
    }
}
