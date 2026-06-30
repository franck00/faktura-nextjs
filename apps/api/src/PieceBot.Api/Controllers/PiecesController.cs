using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Pièces justificatives (spec §6.4). Passe par la couche BLL
/// (<see cref="IPieceService"/>). Le tenant et l'utilisateur validateur seront
/// résolus depuis le JWT Clerk ; constantes en attendant.
/// </summary>
[ApiController]
[Route("api/pieces")]
public sealed class PiecesController : ControllerBase
{
    // TODO: résoudre tenant + utilisateur depuis le JWT Clerk.
    private const string TenantId = "tenant_mvogo";
    private const string ValidatorUserId = "clerk_user_id";

    private readonly IPieceService _service;

    public PiecesController(IPieceService service)
    {
        _service = service;
    }

    /// <summary>GET /api/pieces?month=&amp;endClientId=&amp;status= — liste filtrée.</summary>
    [HttpGet]
    public async Task<ActionResult> List(
        [FromQuery] string? month,
        [FromQuery] string? endClientId,
        [FromQuery] PieceStatus? status,
        CancellationToken cancellationToken)
    {
        var items = await _service.ListAsync(TenantId, month, endClientId, status, cancellationToken);
        return Ok(new { items, total = items.Count });
    }

    /// <summary>GET /api/pieces/{id} — détail d'une pièce.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Piece>> Get(string id, CancellationToken cancellationToken)
    {
        var piece = await _service.GetAsync(TenantId, id, cancellationToken);
        return piece is null ? NotFound(new { error = "Pièce introuvable" }) : Ok(piece);
    }

    /// <summary>GET /api/pieces/{id}/download — URL signée temporaire du blob.</summary>
    [HttpGet("{id}/download")]
    public async Task<ActionResult> Download(string id, CancellationToken cancellationToken)
    {
        var piece = await _service.GetAsync(TenantId, id, cancellationToken);
        if (piece is null)
        {
            return NotFound(new { error = "Pièce introuvable" });
        }

        // TODO: générer une URL SAS signée (Blob Storage). En attendant, URL brute.
        return Ok(new { url = piece.BlobUrl, expiresAt = DateTimeOffset.UtcNow.AddMinutes(15) });
    }

    /// <summary>PUT /api/pieces/{id}/validate — valide l'extraction.</summary>
    [HttpPut("{id}/validate")]
    public async Task<ActionResult<Piece>> Validate(
        string id,
        [FromBody] ValidatePieceRequest? request,
        CancellationToken cancellationToken)
    {
        var validatedBy = string.IsNullOrWhiteSpace(request?.ValidatedBy)
            ? ValidatorUserId
            : request.ValidatedBy;

        var piece = await _service.ValidateAsync(TenantId, id, validatedBy, cancellationToken);
        return piece is null ? NotFound(new { error = "Pièce introuvable" }) : Ok(piece);
    }

    /// <summary>PUT /api/pieces/{id} — corrige catégorie et/ou données extraites.</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Piece>> Correct(
        string id,
        [FromBody] CorrectPieceRequest request,
        CancellationToken cancellationToken)
    {
        var piece = await _service.CorrectAsync(
            TenantId, id, request.Category, request.ExtractedData, cancellationToken);
        return piece is null ? NotFound(new { error = "Pièce introuvable" }) : Ok(piece);
    }

    /// <summary>DELETE /api/pieces/{id} — supprime une pièce.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(TenantId, id, cancellationToken);
        return deleted ? NoContent() : NotFound(new { error = "Pièce introuvable" });
    }
}
