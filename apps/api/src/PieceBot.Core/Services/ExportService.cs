using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>Implémentation BLL des exports mensuels (spec §5.7 / §6.5).</summary>
public sealed class ExportService : IExportService
{
    private readonly IExportJobRepository _jobs;
    private readonly IPieceRepository _pieces;

    public ExportService(IExportJobRepository jobs, IPieceRepository pieces)
    {
        _jobs = jobs;
        _pieces = pieces;
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

            // TODO: rendu réel du fichier (QuestPDF pour PDF, ClosedXML pour Excel,
            // format compatible Sage/Saari/Ciel) puis upload vers Blob Storage avec
            // une URL SAS signée. En attendant, on calcule un nom de fichier et une
            // URL blob déterministe.
            var ext = format == ExportFormat.Pdf ? "pdf" : "xlsx";
            var scope = endClientId ?? "all";
            job.FileName = $"piecebot_{month}_{scope}.{ext}";
            job.BlobUrl =
                $"https://piecebot.blob.core.windows.net/exports/{tenantId}/{month}/{job.Id}.{ext}";

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
}
