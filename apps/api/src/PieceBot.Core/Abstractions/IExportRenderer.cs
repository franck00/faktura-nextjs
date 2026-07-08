using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Rendu réel d'un dossier mensuel en fichier (PDF via QuestPDF, Excel via
/// ClosedXML). L'implémentation vit dans l'infrastructure ; la BLL n'en connaît
/// que ce contrat, ce qui garde <c>PieceBot.Core</c> sans dépendance de rendu.
/// </summary>
public interface IExportRenderer
{
    /// <param name="job">Job d'export (format, mois, périmètre, agrégats).</param>
    /// <param name="pieces">Pièces à inclure, déjà filtrées par mois/client.</param>
    /// <param name="clientNames">Map endClientId → nom d'entreprise pour l'affichage.</param>
    ExportFile Render(
        ExportJob job,
        IReadOnlyList<Piece> pieces,
        IReadOnlyDictionary<string, string> clientNames);
}
