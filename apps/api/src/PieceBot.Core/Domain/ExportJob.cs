namespace PieceBot.Core.Domain;

/// <summary>Format d'export d'un dossier mensuel (spec §6.5).</summary>
public enum ExportFormat
{
    Pdf,
    Excel
}

/// <summary>Cycle de vie d'un job d'export asynchrone (spec §6.5).</summary>
public enum ExportStatus
{
    Queued,
    Processing,
    Completed,
    Failed
}

/// <summary>
/// Job d'export d'un dossier mensuel (spec §5.7 / §6.5). Le pattern est
/// asynchrone : <c>POST</c> crée le job, <c>GET /api/exports/{jobId}</c> renvoie
/// le statut puis l'URL de téléchargement. Scopé par <see cref="TenantId"/>.
/// </summary>
public sealed class ExportJob
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public string Type { get; init; } = "export";

    public ExportFormat Format { get; init; }

    /// <summary>Mois exporté, format <c>YYYY-MM</c>.</summary>
    public required string Month { get; init; }

    /// <summary>Client ciblé, ou <c>null</c> pour l'export par lot (tous les clients).</summary>
    public string? EndClientId { get; init; }

    public ExportStatus Status { get; set; } = ExportStatus.Queued;

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>URL (SAS signée) du fichier généré, une fois le job terminé.</summary>
    public string? BlobUrl { get; set; }
    public string? FileName { get; set; }

    /// <summary>Nombre de pièces incluses dans l'export.</summary>
    public int PieceCount { get; set; }

    /// <summary>Total TTC agrégé des pièces exportées (plus petite unité de devise).</summary>
    public long TotalAmountTtc { get; set; }

    public string? Error { get; set; }
}
