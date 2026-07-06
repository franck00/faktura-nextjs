using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL des exports mensuels (spec §5.7 / §6.5). Crée des jobs d'export
/// (PDF / Excel) pour un dossier mensuel — un client précis ou tout le cabinet
/// (export par lot). Le rendu de fichier réel (QuestPDF / ClosedXML) et l'upload
/// Blob seront branchés dans l'infrastructure ; ici on agrège les données.
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Crée (et traite) un job d'export du mois pour un client donné, ou pour
    /// tous les clients si <paramref name="endClientId"/> est <c>null</c>.
    /// </summary>
    Task<ExportJob> CreateMonthlyExportAsync(
        string tenantId,
        ExportFormat format,
        string month,
        string? endClientId = null,
        CancellationToken cancellationToken = default);

    Task<ExportJob?> GetAsync(
        string tenantId,
        string jobId,
        CancellationToken cancellationToken = default);
}
