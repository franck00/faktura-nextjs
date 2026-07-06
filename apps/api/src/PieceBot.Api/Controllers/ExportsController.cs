using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Exports de dossiers mensuels (spec §6.5). Pattern asynchrone : <c>POST</c>
/// crée un job, <c>GET /api/exports/{jobId}</c> renvoie le statut puis l'URL de
/// téléchargement. Passe par la couche BLL (<see cref="IExportService"/>).
/// </summary>
[ApiController]
[Route("api/exports")]
public sealed class ExportsController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk (claim) au lieu d'une constante.
    private const string TenantId = "tenant_mvogo";

    private readonly IExportService _service;

    public ExportsController(IExportService service)
    {
        _service = service;
    }

    /// <summary>POST /api/exports/monthly-pdf — génère le dossier mensuel en PDF.</summary>
    [HttpPost("monthly-pdf")]
    public Task<ActionResult<ExportJob>> MonthlyPdf(
        [FromBody] CreateExportRequest request,
        CancellationToken cancellationToken) =>
        CreateExport(ExportFormat.Pdf, request, cancellationToken);

    /// <summary>POST /api/exports/monthly-excel — génère le dossier mensuel en Excel.</summary>
    [HttpPost("monthly-excel")]
    public Task<ActionResult<ExportJob>> MonthlyExcel(
        [FromBody] CreateExportRequest request,
        CancellationToken cancellationToken) =>
        CreateExport(ExportFormat.Excel, request, cancellationToken);

    /// <summary>GET /api/exports/{jobId} — statut du job + URL de téléchargement.</summary>
    [HttpGet("{jobId}")]
    public async Task<ActionResult<ExportJob>> Get(string jobId, CancellationToken cancellationToken)
    {
        var job = await _service.GetAsync(TenantId, jobId, cancellationToken);
        return job is null ? NotFound(new { error = "Job d'export introuvable" }) : Ok(job);
    }

    private async Task<ActionResult<ExportJob>> CreateExport(
        ExportFormat format,
        CreateExportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var job = await _service.CreateMonthlyExportAsync(
                TenantId, format, request.Month, request.EndClientId, cancellationToken);

            return AcceptedAtAction(nameof(Get), new { jobId = job.Id }, job);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
