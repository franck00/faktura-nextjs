using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Stockage des binaires de pièces téléchargés depuis WhatsApp, adressés par une
/// clé <c>tenant/mois/media.ext</c>. In-Memory pour l'instant ; remplaçable par
/// Azure Blob (URL SAS) sans impact sur la BLL. Servi via <c>GET /api/media/{key}</c>.
/// </summary>
public interface IMediaStore
{
    Task SaveAsync(string key, byte[] bytes, string contentType, string fileName, CancellationToken cancellationToken = default);

    Task<MediaFile?> GetAsync(string key, CancellationToken cancellationToken = default);
}
