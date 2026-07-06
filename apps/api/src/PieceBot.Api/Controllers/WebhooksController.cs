using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Webhooks publics WhatsApp (spec §6.1). Non authentifiés côté Clerk : la
/// sécurité repose sur le token de vérification (GET) et la signature HMAC (POST).
/// </summary>
[ApiController]
[Route("api/webhooks/whatsapp")]
public sealed class WebhooksController : ControllerBase
{
    private readonly IWhatsAppWebhookService _service;
    private readonly IConfiguration _configuration;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WebhooksController(IWhatsAppWebhookService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    /// <summary>
    /// GET /api/webhooks/whatsapp/verify — handshake de vérification Meta.
    /// Renvoie le challenge en clair si le token correspond.
    /// </summary>
    [HttpGet("verify")]
    public IActionResult Verify(
        [FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? verifyToken,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        var expected = _configuration["WhatsApp:VerifyToken"] ?? string.Empty;
        var result = _service.HandleVerification(mode, verifyToken, challenge, expected);
        return result is null
            ? Unauthorized(new { error = "Vérification échouée" })
            : Content(result, "text/plain");
    }

    /// <summary>
    /// POST /api/webhooks/whatsapp — réception des messages entrants. Valide la
    /// signature HMAC (si un App Secret est configuré), puis traite chaque message.
    /// Répond toujours 200 rapidement (exigence Meta).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Receive(CancellationToken cancellationToken)
    {
        // Lire le corps brut (nécessaire pour la vérification HMAC).
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var bodyBytes = System.Text.Encoding.UTF8.GetBytes(rawBody);

        var appSecret = _configuration["WhatsApp:AppSecret"];
        if (!string.IsNullOrEmpty(appSecret))
        {
            var signature = Request.Headers["X-Hub-Signature-256"].FirstOrDefault();
            if (!_service.VerifySignature(bodyBytes, signature, appSecret))
            {
                return Unauthorized(new { error = "Signature invalide" });
            }
        }

        WhatsAppWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<WhatsAppWebhookPayload>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            return BadRequest(new { error = "Payload invalide" });
        }

        if (payload is null)
        {
            return Ok(new { processed = 0 });
        }

        var results = new List<object>();
        foreach (var (context, message) in payload.ExtractMessages())
        {
            var result = await _service.ProcessAsync(context, message, cancellationToken);
            results.Add(new { messageId = message.MessageId, outcome = result.Outcome, pieceId = result.PieceId });
        }

        return Ok(new { processed = results.Count, results });
    }
}
