using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Messaging;

/// <summary>
/// Implémentation provisoire de <see cref="IWhatsAppMediaStore"/> : ne télécharge
/// pas encore le binaire Meta et ne pousse pas vers Blob Storage — renvoie une URL
/// déterministe, miroir de la convention <c>tenant/mois/piece.ext</c>. À remplacer
/// par le vrai téléchargement Graph API + upload Blob.
/// </summary>
public sealed class StubWhatsAppMediaStore : IWhatsAppMediaStore
{
    public Task<StoredMedia> DownloadAndStoreAsync(
        string mediaId,
        string tenantId,
        string month,
        string mimeType,
        string? suggestedFileName,
        CancellationToken cancellationToken = default)
    {
        var ext = ExtensionFor(mimeType);
        var fileName = string.IsNullOrWhiteSpace(suggestedFileName)
            ? $"{mediaId}.{ext}"
            : suggestedFileName;
        var blobUrl =
            $"https://piecebot.blob.core.windows.net/pieces/{tenantId}/{month}/{mediaId}.{ext}";

        return Task.FromResult(new StoredMedia(blobUrl, mimeType, fileName));
    }

    private static string ExtensionFor(string mimeType) => mimeType switch
    {
        "image/jpeg" => "jpg",
        "image/png" => "png",
        "image/webp" => "webp",
        "application/pdf" => "pdf",
        _ => "bin"
    };
}
