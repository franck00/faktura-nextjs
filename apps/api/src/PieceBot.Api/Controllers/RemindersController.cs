using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Rappels mensuels (spec §6.7). Passe par la couche BLL
/// (<see cref="IReminderService"/>). Le tenant sera résolu depuis le JWT Clerk ;
/// constante <c>TenantId</c> en attendant.
/// </summary>
[ApiController]
[Route("api/reminders")]
public sealed class RemindersController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk (claim) au lieu d'une constante.
    private const string TenantId = "tenant_mvogo";

    private readonly IReminderService _service;

    public RemindersController(IReminderService service)
    {
        _service = service;
    }

    /// <summary>GET /api/reminders/config — configuration actuelle des rappels.</summary>
    [HttpGet("config")]
    public async Task<ActionResult<ReminderConfig>> GetConfig(CancellationToken cancellationToken)
    {
        var config = await _service.GetConfigAsync(TenantId, cancellationToken);
        return config is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(config);
    }

    /// <summary>PUT /api/reminders/config — met à jour la configuration des rappels.</summary>
    [HttpPut("config")]
    public async Task<ActionResult<ReminderConfig>> UpdateConfig(
        [FromBody] UpdateReminderConfigRequest request,
        CancellationToken cancellationToken)
    {
        var config = new ReminderConfig
        {
            MonthlyReminderDay = request.MonthlyReminderDay,
            MonthlyReminderHour = request.MonthlyReminderHour,
            Enabled = request.Enabled,
            MessageTemplate = request.MessageTemplate ?? string.Empty
        };

        try
        {
            var updated = await _service.UpdateConfigAsync(TenantId, config, cancellationToken);
            return updated is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>POST /api/reminders/send-now — déclenche manuellement les rappels du mois.</summary>
    [HttpPost("send-now")]
    public async Task<ActionResult<ReminderRunResult>> SendNow(
        [FromBody] SendRemindersRequest? request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SendNowAsync(TenantId, request?.Month, cancellationToken);
        return result is null ? NotFound(new { error = "Cabinet introuvable" }) : Ok(result);
    }
}
