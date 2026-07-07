using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL du webhook WhatsApp (spec §5.3 / §6.1). Gère le handshake de
/// vérification Meta, la validation de signature HMAC, et le traitement des
/// messages entrants (identification tenant/client, stockage média, création de
/// pièce, accusé de réception).
/// </summary>
public interface IWhatsAppWebhookService
{
    /// <summary>
    /// Handshake de vérification Meta (GET). Renvoie le <paramref name="challenge"/>
    /// si <paramref name="mode"/> = <c>subscribe</c> et que le token correspond,
    /// sinon <c>null</c>.
    /// </summary>
    string? HandleVerification(
        string? mode,
        string? verifyToken,
        string? challenge,
        string expectedVerifyToken);

    /// <summary>
    /// Valide la signature <c>X-Hub-Signature-256</c> (HMAC SHA-256 du corps brut
    /// avec l'App Secret). Comparaison à temps constant.
    /// </summary>
    bool VerifySignature(byte[] rawBody, string? signatureHeader, string appSecret);

    /// <summary>Traite un message entrant normalisé.</summary>
    Task<WebhookProcessResult> ProcessAsync(
        WhatsAppWebhookContext context,
        WhatsAppInboundMessage message,
        CancellationToken cancellationToken = default);
}
