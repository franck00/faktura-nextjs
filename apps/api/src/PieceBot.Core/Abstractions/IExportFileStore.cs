using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Stockage des fichiers d'export générés, scopé par tenant. Implémentation
/// In-Memory pour l'instant ; remplaçable par Azure Blob (URL SAS) sans toucher
/// à la BLL. Le téléchargement passe par <c>GET /api/exports/{jobId}/download</c>.
/// </summary>
public interface IExportFileStore
{
    Task SaveAsync(string tenantId, string jobId, ExportFile file, CancellationToken cancellationToken = default);

    Task<ExportFile?> OpenAsync(string tenantId, string jobId, CancellationToken cancellationToken = default);
}
