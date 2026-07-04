using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Port de téléchargement des médias WhatsApp (spec §3.1). L'implémentation réelle
/// (PieceBot.Infrastructure) télécharge le binaire via la Graph API Meta puis
/// l'envoie dans Azure Blob Storage sous <c>tenant/mois/piece.ext</c>.
/// </summary>
public interface IWhatsAppMediaStore
{
    Task<StoredMedia> DownloadAndStoreAsync(
        string mediaId,
        string tenantId,
        string month,
        string mimeType,
        string? suggestedFileName,
        CancellationToken cancellationToken = default);
}
