namespace PieceBot.Core.Domain;

/// <summary>Données structurées extraites par l'OCR (cf. spec §4.3).</summary>
public sealed class ExtractedData
{
    public string? Supplier { get; set; }
    public DateOnly? DocumentDate { get; set; }
    public string? DocumentNumber { get; set; }
    public long? TotalAmountTtc { get; set; }
    public long? TotalAmountHt { get; set; }
    public long? TotalVat { get; set; }
    public string? Currency { get; set; }
}

/// <summary>
/// Pièce justificative reçue par WhatsApp (collection Cosmos <c>pieces</c>,
/// spec §4.3). Partitionnée par <see cref="TenantId"/>.
/// </summary>
public sealed class Piece
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public required string EndClientId { get; init; }
    public string Type { get; init; } = "piece";

    public PieceCategory Category { get; set; }
    public PieceStatus Status { get; set; }

    /// <summary>Mois comptable de rattachement, format <c>YYYY-MM</c>.</summary>
    public required string Month { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }

    public required string BlobUrl { get; set; }
    public required string MimeType { get; init; }
    public required string OriginalFileName { get; init; }

    public ExtractedData ExtractedData { get; set; } = new();

    /// <summary>Score de confiance global de l'extraction, 0..1.</summary>
    public double Confidence { get; set; }

    public string? ValidatedBy { get; set; }
    public DateTimeOffset? ValidatedAt { get; set; }
    public string? OcrRawText { get; set; }
    public required string WhatsappMessageId { get; init; }
}
