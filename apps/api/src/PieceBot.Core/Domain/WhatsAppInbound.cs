namespace PieceBot.Core.Domain;

/// <summary>Type de message WhatsApp entrant (normalisé, indépendant du format Meta).</summary>
public enum WhatsAppMessageKind
{
    Text,
    Image,
    Document,
    Audio,
    Video,
    Unsupported
}

/// <summary>
/// Message WhatsApp entrant, normalisé depuis le payload Meta (spec §3.1 / §5.3).
/// </summary>
public sealed class WhatsAppInboundMessage
{
    /// <summary>Numéro expéditeur tel que fourni par Meta (ex. <c>237699123456</c>).</summary>
    public required string From { get; init; }

    /// <summary>Identifiant du message Meta (<c>wamid.xxx</c>) — idempotence.</summary>
    public required string MessageId { get; init; }

    public DateTimeOffset Timestamp { get; init; }
    public WhatsAppMessageKind Kind { get; init; }

    public string? Text { get; init; }
    public string? Caption { get; init; }

    public string? MediaId { get; init; }
    public string? MimeType { get; init; }
    public string? FileName { get; init; }

    public bool HasMedia => !string.IsNullOrEmpty(MediaId);
}

/// <summary>Contexte d'un lot de messages (identifie le tenant via le Phone Number ID).</summary>
public sealed class WhatsAppWebhookContext
{
    public required string PhoneNumberId { get; init; }
    public string? ContactName { get; init; }
}

/// <summary>Issue du traitement d'un message entrant.</summary>
public enum WebhookOutcome
{
    Ignored,
    TenantNotFound,
    UnknownSender,
    PieceCreated,
    Acknowledged
}

/// <summary>Résultat du traitement d'un message par la BLL.</summary>
public sealed class WebhookProcessResult
{
    public required WebhookOutcome Outcome { get; init; }
    public string? PieceId { get; init; }
    public string? AckMessage { get; init; }

    public static WebhookProcessResult Ignored() => new() { Outcome = WebhookOutcome.Ignored };
}

/// <summary>
/// Média téléchargé depuis Meta puis stocké. <see cref="StorageKey"/> permet à la
/// BLL de récupérer le binaire (via <see cref="Abstractions.IMediaStore"/>) pour
/// l'OCR ; <c>null</c> si le média n'est pas réellement stocké (stub).
/// </summary>
public sealed record StoredMedia(string BlobUrl, string MimeType, string FileName, string? StorageKey = null);
