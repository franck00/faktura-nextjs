using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Extraction OCR d'une pièce (spec §3.2 / §5.4). L'implémentation réelle appelle
/// Azure Document Intelligence ; un no-op renvoie un résultat vide tant que l'OCR
/// n'est pas configuré. La BLL ne dépend que de ce contrat.
/// </summary>
public interface IReceiptExtractor
{
    Task<ReceiptExtraction> ExtractAsync(
        byte[] content,
        string mimeType,
        CancellationToken cancellationToken = default);
}
