namespace PieceBot.Core.Domain;

/// <summary>
/// Résultat d'extraction OCR d'une pièce : données structurées, score de
/// confiance global (0..1) et catégorie devinée. Produit par un
/// <see cref="Abstractions.IReceiptExtractor"/>.
/// </summary>
public sealed record ReceiptExtraction(ExtractedData Data, double Confidence, PieceCategory Category)
{
    /// <summary>Résultat vide (aucune extraction) — utilisé sans OCR configuré.</summary>
    public static ReceiptExtraction Empty { get; } =
        new(new ExtractedData(), 0, PieceCategory.Other);
}
