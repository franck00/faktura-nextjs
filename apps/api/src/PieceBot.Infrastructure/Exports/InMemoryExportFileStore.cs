using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Exports;

/// <summary>
/// Stockage In-Memory des fichiers d'export générés (démo / dev). À remplacer
/// par Azure Blob Storage (URL SAS) sans impact sur la BLL. Scopé par tenant.
/// </summary>
public sealed class InMemoryExportFileStore : IExportFileStore
{
    private readonly ConcurrentDictionary<string, ExportFile> _files = new();

    private static string Key(string tenantId, string jobId) => $"{tenantId}/{jobId}";

    public Task SaveAsync(string tenantId, string jobId, ExportFile file, CancellationToken cancellationToken = default)
    {
        _files[Key(tenantId, jobId)] = file;
        return Task.CompletedTask;
    }

    public Task<ExportFile?> OpenAsync(string tenantId, string jobId, CancellationToken cancellationToken = default)
    {
        _files.TryGetValue(Key(tenantId, jobId), out var file);
        return Task.FromResult(file);
    }
}
