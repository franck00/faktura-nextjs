using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Ocr;

/// <summary>
/// Extracteur par défaut quand Azure Document Intelligence n'est pas configuré :
/// ne fait rien (la pièce reste à traiter manuellement). Remplacé par
/// <see cref="AzureDocumentIntelligenceExtractor"/> dès que l'OCR est branché.
/// </summary>
public sealed class NoOpReceiptExtractor : IReceiptExtractor
{
    public Task<ReceiptExtraction> ExtractAsync(
        byte[] content,
        string mimeType,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ReceiptExtraction.Empty);
}
