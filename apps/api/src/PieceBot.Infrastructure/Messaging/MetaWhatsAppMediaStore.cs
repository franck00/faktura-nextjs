using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Messaging;

/// <summary>
/// Téléchargement réel des médias WhatsApp via la Graph API Meta (spec §3.1),
/// en deux temps : récupération de l'URL signée du média, puis téléchargement du
/// binaire (Bearer requis). Le binaire est rangé dans <see cref="IMediaStore"/>
/// et servi par <c>GET /api/media/{key}</c>. Activé quand <c>WhatsApp:AccessToken</c>
/// est configuré (sinon <see cref="StubWhatsAppMediaStore"/>).
/// </summary>
public sealed class MetaWhatsAppMediaStore : IWhatsAppMediaStore
{
    private readonly HttpClient _http;
    private readonly IMediaStore _store;
    private readonly string _accessToken;
    private readonly string _apiVersion;

    public MetaWhatsAppMediaStore(HttpClient http, IMediaStore store, IConfiguration configuration)
    {
        _http = http;
        _store = store;
        _accessToken = configuration["WhatsApp:AccessToken"] ?? string.Empty;
        _apiVersion = configuration["WhatsApp:GraphApiVersion"] is { Length: > 0 } v ? v : "v21.0";
    }

    public async Task<StoredMedia> DownloadAndStoreAsync(
        string mediaId,
        string tenantId,
        string month,
        string mimeType,
        string? suggestedFileName,
        CancellationToken cancellationToken = default)
    {
        // 1. Métadonnées du média (URL signée temporaire + type réel).
        var meta = await GetWithAuthAsync<MediaMetadata>(
            $"https://graph.facebook.com/{_apiVersion}/{mediaId}", cancellationToken);

        var downloadUrl = meta?.Url
            ?? throw new InvalidOperationException($"URL du média {mediaId} introuvable (réponse Meta).");

        var effectiveMime = !string.IsNullOrWhiteSpace(mimeType)
            ? mimeType
            : meta.MimeType ?? "application/octet-stream";

        // 2. Téléchargement du binaire (l'URL Meta exige aussi le Bearer).
        using var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        using var response = await _http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        // 3. Rangement + URL de service.
        var ext = ExtensionFor(effectiveMime);
        var fileName = string.IsNullOrWhiteSpace(suggestedFileName) ? $"{mediaId}.{ext}" : suggestedFileName;
        var key = $"{tenantId}/{month}/{mediaId}.{ext}";
        await _store.SaveAsync(key, bytes, effectiveMime, fileName, cancellationToken);

        return new StoredMedia($"/api/media/{key}", effectiveMime, fileName, key);
    }

    private async Task<T?> GetWithAuthAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        using var response = await _http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private static string ExtensionFor(string mimeType) => mimeType switch
    {
        "image/jpeg" => "jpg",
        "image/png" => "png",
        "image/webp" => "webp",
        "application/pdf" => "pdf",
        _ => "bin"
    };

    private sealed record MediaMetadata(
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("mime_type")] string? MimeType);
}
