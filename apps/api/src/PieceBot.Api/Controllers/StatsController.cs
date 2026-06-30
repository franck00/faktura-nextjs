using Microsoft.AspNetCore.Mvc;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Statistiques dashboard (spec §6.6). S'appuie sur la couche BLL
/// (<see cref="IStatsService"/>). Mois par défaut = mois courant.
/// </summary>
[ApiController]
[Route("api/stats")]
public sealed class StatsController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk.
    private const string TenantId = "tenant_mvogo";

    private readonly IStatsService _service;

    public StatsController(IStatsService service)
    {
        _service = service;
    }

    /// <summary>GET /api/stats/dashboard?month=2026-06 — KPIs du dashboard.</summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStats>> Dashboard(
        [FromQuery] string? month,
        CancellationToken cancellationToken)
    {
        var resolvedMonth = ResolveMonth(month);
        var stats = await _service.GetDashboardAsync(TenantId, resolvedMonth, cancellationToken);
        return Ok(stats);
    }

    /// <summary>GET /api/stats/completion?month=2026-06 — taux de complétion par client.</summary>
    [HttpGet("completion")]
    public async Task<ActionResult> Completion(
        [FromQuery] string? month,
        CancellationToken cancellationToken)
    {
        var resolvedMonth = ResolveMonth(month);
        var items = await _service.GetCompletionAsync(TenantId, resolvedMonth, cancellationToken);
        return Ok(new { month = resolvedMonth, items });
    }

    private static string ResolveMonth(string? month) =>
        string.IsNullOrWhiteSpace(month) ? DateTimeOffset.UtcNow.ToString("yyyy-MM") : month;
}
