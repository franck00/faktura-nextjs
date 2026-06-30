using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Cabinet courant (spec §6.2). Passe par la couche BLL
/// (<see cref="ITenantService"/>). Le tenant sera résolu depuis le JWT Clerk ;
/// constante <c>TenantId</c> en attendant.
/// </summary>
[ApiController]
[Route("api/tenants")]
public sealed class TenantsController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk (claim) au lieu d'une constante.
    private const string TenantId = "tenant_mvogo";

    private readonly ITenantService _service;

    public TenantsController(ITenantService service)
    {
        _service = service;
    }

    /// <summary>GET /api/tenants/me — informations du cabinet courant.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<Tenant>> Me(CancellationToken cancellationToken)
    {
        var tenant = await _service.GetAsync(TenantId, cancellationToken);
        return tenant is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(tenant);
    }

    /// <summary>PUT /api/tenants/me — met à jour le cabinet courant.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<Tenant>> Update(
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await _service.GetAsync(TenantId, cancellationToken);
        if (tenant is null)
        {
            return NotFound(new { error = "Cabinet introuvable" });
        }

        tenant.Name = request.Name;
        tenant.Country = request.Country;
        tenant.Currency = request.Currency;
        tenant.VatRate = request.VatRate;

        if (request.Settings is { } s)
        {
            tenant.Settings = new TenantSettings
            {
                Language = s.Language,
                Timezone = s.Timezone,
                MonthlyReminderDay = s.MonthlyReminderDay,
                MonthlyReminderHour = s.MonthlyReminderHour
            };
        }

        try
        {
            var updated = await _service.UpdateAsync(tenant, cancellationToken);
            return updated is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>POST /api/tenants/me/whatsapp-link — lie le numéro WhatsApp Business.</summary>
    [HttpPost("me/whatsapp-link")]
    public async Task<ActionResult<Tenant>> LinkWhatsapp(
        [FromBody] LinkWhatsappRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.LinkWhatsappAsync(
                TenantId,
                request.PhoneNumberId,
                request.BusinessAccountId ?? string.Empty,
                cancellationToken);

            return updated is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
