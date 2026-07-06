using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Accès aux jobs d'export mensuel (spec §6.5). Toujours scopé par
/// <paramref name="tenantId"/>. L'implémentation Cosmos vivra dans
/// PieceBot.Infrastructure.
/// </summary>
public interface IExportJobRepository
{
    Task<ExportJob> CreateAsync(ExportJob job, CancellationToken cancellationToken = default);

    Task<ExportJob?> GetAsync(
        string tenantId,
        string jobId,
        CancellationToken cancellationToken = default);

    Task<ExportJob?> UpdateAsync(ExportJob job, CancellationToken cancellationToken = default);
}
