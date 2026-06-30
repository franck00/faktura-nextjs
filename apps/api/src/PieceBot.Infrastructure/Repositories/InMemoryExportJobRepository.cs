using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Repositories;

/// <summary>
/// Implémentation mémoire provisoire de <see cref="IExportJobRepository"/>.
/// Démarre vide (les jobs sont créés à la demande). À remplacer par un
/// repository Cosmos DB (partition tenantId) sans changer l'interface.
/// </summary>
public sealed class InMemoryExportJobRepository : IExportJobRepository
{
    private readonly ConcurrentDictionary<string, ExportJob> _jobs = new();

    public Task<ExportJob> CreateAsync(ExportJob job, CancellationToken cancellationToken = default)
    {
        _jobs[job.Id] = job;
        return Task.FromResult(job);
    }

    public Task<ExportJob?> GetAsync(
        string tenantId,
        string jobId,
        CancellationToken cancellationToken = default)
    {
        _jobs.TryGetValue(jobId, out var job);
        return Task.FromResult(job is not null && job.TenantId == tenantId ? job : null);
    }

    public Task<ExportJob?> UpdateAsync(ExportJob job, CancellationToken cancellationToken = default)
    {
        if (!_jobs.ContainsKey(job.Id))
        {
            return Task.FromResult<ExportJob?>(null);
        }

        _jobs[job.Id] = job;
        return Task.FromResult<ExportJob?>(job);
    }
}
