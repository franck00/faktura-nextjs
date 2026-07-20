using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using PieceBot.Core.Abstractions;

namespace PieceBot.Infrastructure.Messaging;

/// <summary>
/// Envoi réel de messages WhatsApp via la Meta Cloud API (spec §5.3) : sert à
/// renvoyer l'accusé de réception au client. Activé quand <c>WhatsApp:AccessToken</c>
/// est configuré (sinon <see cref="StubWhatsAppSender"/>).
/// </summary>
public sealed class MetaWhatsAppSender : IWhatsAppSender
{
    private readonly HttpClient _http;
    private readonly string _accessToken;
    private readonly string _apiVersion;

    public MetaWhatsAppSender(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _accessToken = configuration["WhatsApp:AccessToken"] ?? string.Empty;
        _apiVersion = configuration["WhatsApp:GraphApiVersion"] is { Length: > 0 } v ? v : "v21.0";
    }

    public async Task SendTextAsync(
        string phoneNumberId,
        string toNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        var url = $"https://graph.facebook.com/{_apiVersion}/{phoneNumberId}/messages";

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = toNumber,
                type = "text",
                text = new { preview_url = false, body = message }
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        using var response = await _http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
