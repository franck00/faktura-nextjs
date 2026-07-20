using System.Security.Cryptography;
using System.Text;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>
/// Implémentation BLL du webhook WhatsApp (spec §5.3). Orchestration :
/// identifie le cabinet (tenant) via le Phone Number ID, puis le client via le
/// numéro expéditeur ; télécharge/stocke le média ; crée la pièce ; renvoie un
/// accusé de réception envoyé au client.
/// </summary>
public sealed class WhatsAppWebhookService : IWhatsAppWebhookService
{
    private readonly ITenantRepository _tenants;
    private readonly IEndClientRepository _clients;
    private readonly IPieceRepository _pieces;
    private readonly IWhatsAppMediaStore _mediaStore;
    private readonly IMediaStore _mediaBinaries;
    private readonly IReceiptExtractor _extractor;
    private readonly IWhatsAppSender _sender;

    public WhatsAppWebhookService(
        ITenantRepository tenants,
        IEndClientRepository clients,
        IPieceRepository pieces,
        IWhatsAppMediaStore mediaStore,
        IMediaStore mediaBinaries,
        IReceiptExtractor extractor,
        IWhatsAppSender sender)
    {
        _tenants = tenants;
        _clients = clients;
        _pieces = pieces;
        _mediaStore = mediaStore;
        _mediaBinaries = mediaBinaries;
        _extractor = extractor;
        _sender = sender;
    }

    public string? HandleVerification(
        string? mode,
        string? verifyToken,
        string? challenge,
        string expectedVerifyToken)
    {
        if (mode == "subscribe"
            && !string.IsNullOrEmpty(expectedVerifyToken)
            && verifyToken == expectedVerifyToken)
        {
            return challenge;
        }

        return null;
    }

    public bool VerifySignature(byte[] rawBody, string? signatureHeader, string appSecret)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(appSecret))
        {
            return false;
        }

        // En-tête au format "sha256=<hex>".
        const string prefix = "sha256=";
        if (!signatureHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var provided = signatureHeader[prefix.Length..];
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var computed = Convert.ToHexStringLower(hmac.ComputeHash(rawBody));

        // Comparaison à temps constant.
        var providedBytes = Encoding.ASCII.GetBytes(provided);
        var computedBytes = Encoding.ASCII.GetBytes(computed);
        return CryptographicOperations.FixedTimeEquals(providedBytes, computedBytes);
    }

    public async Task<WebhookProcessResult> ProcessAsync(
        WhatsAppWebhookContext context,
        WhatsAppInboundMessage message,
        CancellationToken cancellationToken = default)
    {
        // 1. Identifier le cabinet via le Phone Number ID Meta.
        var tenant = await _tenants.GetByWhatsappPhoneNumberIdAsync(
            context.PhoneNumberId, cancellationToken);
        if (tenant is null)
        {
            return new WebhookProcessResult { Outcome = WebhookOutcome.TenantNotFound };
        }

        // 2. Identifier le client via le numéro expéditeur.
        var client = await _clients.GetByWhatsappNumberAsync(
            tenant.TenantId, message.From, cancellationToken);
        if (client is null)
        {
            var greeting =
                $"Bonjour, je suis l'assistant de {tenant.Name}. " +
                "Je ne reconnais pas votre numéro — pouvez-vous indiquer votre nom et votre entreprise ?";
            await _sender.SendTextAsync(context.PhoneNumberId, message.From, greeting, cancellationToken);
            return new WebhookProcessResult
            {
                Outcome = WebhookOutcome.UnknownSender,
                AckMessage = greeting
            };
        }

        // 3a. Message média → création d'une pièce.
        if (message.HasMedia)
        {
            var month = message.Timestamp.ToString("yyyy-MM");
            var stored = await _mediaStore.DownloadAndStoreAsync(
                message.MediaId!,
                tenant.TenantId,
                month,
                message.MimeType ?? "application/octet-stream",
                message.FileName,
                cancellationToken);

            // OCR : extraction des données (montant, date, fournisseur…) si configuré.
            var extraction = await ExtractAsync(stored, cancellationToken);

            var piece = new Piece
            {
                Id = $"piece_{Guid.NewGuid():N}",
                TenantId = tenant.TenantId,
                EndClientId = client.Id,
                Category = extraction.Category,
                // Extracted si l'OCR a produit un résultat, sinon à traiter (Received).
                Status = extraction.Confidence > 0 ? PieceStatus.Extracted : PieceStatus.Received,
                Month = month,
                ReceivedAt = message.Timestamp,
                BlobUrl = stored.BlobUrl,
                MimeType = stored.MimeType,
                OriginalFileName = stored.FileName,
                ExtractedData = extraction.Data,
                Confidence = extraction.Confidence,
                WhatsappMessageId = message.MessageId
            };
            await _pieces.CreateAsync(piece, cancellationToken);

            client.LastPieceReceived = message.Timestamp;
            await _clients.UpdateAsync(client, cancellationToken);

            var ack = $"✓ Reçu ! Justificatif enregistré pour {client.CompanyName}. Merci 🙏";
            await _sender.SendTextAsync(context.PhoneNumberId, message.From, ack, cancellationToken);

            return new WebhookProcessResult
            {
                Outcome = WebhookOutcome.PieceCreated,
                PieceId = piece.Id,
                AckMessage = ack
            };
        }

        // 3b. Message texte (ou non supporté) → guidage, pas de pièce.
        var guidance =
            "Merci ! Pour enregistrer un justificatif, envoyez-le en photo 📸 ou en PDF.";
        await _sender.SendTextAsync(context.PhoneNumberId, message.From, guidance, cancellationToken);
        return new WebhookProcessResult
        {
            Outcome = WebhookOutcome.Acknowledged,
            AckMessage = guidance
        };
    }

    /// <summary>
    /// Récupère le binaire stocké et lance l'OCR. Renvoie un résultat vide si le
    /// média n'est pas récupérable (stub) ou si l'OCR n'est pas configuré.
    /// </summary>
    private async Task<ReceiptExtraction> ExtractAsync(StoredMedia stored, CancellationToken cancellationToken)
    {
        if (stored.StorageKey is null)
        {
            return ReceiptExtraction.Empty;
        }

        var media = await _mediaBinaries.GetAsync(stored.StorageKey, cancellationToken);
        if (media is null)
        {
            return ReceiptExtraction.Empty;
        }

        return await _extractor.ExtractAsync(media.Bytes, media.ContentType, cancellationToken);
    }
}
