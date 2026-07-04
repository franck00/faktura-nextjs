using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Webhook Stripe public (spec §6.1). Sécurité par signature <c>Stripe-Signature</c>.
/// </summary>
[ApiController]
[Route("api/webhooks/stripe")]
public sealed class StripeWebhookController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IBillingService _billing;
    private readonly IConfiguration _configuration;

    public StripeWebhookController(IBillingService billing, IConfiguration configuration)
    {
        _billing = billing;
        _configuration = configuration;
    }

    /// <summary>POST /api/webhooks/stripe — évènements de cycle de vie d'abonnement.</summary>
    [HttpPost]
    public async Task<IActionResult> Receive(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var bodyBytes = Encoding.UTF8.GetBytes(rawBody);

        var webhookSecret = _configuration["Stripe:WebhookSecret"];
        if (!string.IsNullOrEmpty(webhookSecret))
        {
            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (!_billing.VerifySignature(bodyBytes, signature, webhookSecret))
            {
                return Unauthorized(new { error = "Signature invalide" });
            }
        }

        StripeWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<StripeWebhookPayload>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            return BadRequest(new { error = "Payload invalide" });
        }

        if (payload is null)
        {
            return Ok(new { received = true });
        }

        var result = await _billing.HandleEventAsync(payload.ToDomain(), cancellationToken);
        return Ok(new { received = true, outcome = result.Outcome, tenantId = result.TenantId });
    }
}
