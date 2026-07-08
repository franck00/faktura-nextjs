using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Facturation côté cabinet (spec §5.8). Ouvre le portail client Stripe pour
/// gérer l'abonnement / moyen de paiement.
/// </summary>
[ApiController]
[Route("api/billing")]
public sealed class BillingController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk.
    private string TenantId => HttpContext.GetTenantId();

    private readonly IBillingService _billing;

    public BillingController(IBillingService billing)
    {
        _billing = billing;
    }

    /// <summary>POST /api/billing/portal — crée une session du portail client Stripe.</summary>
    [HttpPost("portal")]
    public async Task<IActionResult> Portal(
        [FromBody] CreatePortalRequest request,
        CancellationToken cancellationToken)
    {
        var returnUrl = string.IsNullOrWhiteSpace(request.ReturnUrl)
            ? "https://app.piecebot.com/dashboard"
            : request.ReturnUrl;

        var url = await _billing.CreatePortalUrlAsync(TenantId, returnUrl, cancellationToken);
        return url is null
            ? Conflict(new { error = "Aucun client Stripe rattaché à ce cabinet" })
            : Ok(new { url });
    }
}
