using Microsoft.AspNetCore.Mvc;
using PieceBot.Api.Contracts;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Clients du cabinet (spec §6.3). CRUD complet via la couche BLL
/// (<see cref="IEndClientService"/>). La résolution du tenant via Clerk
/// remplacera la constante <c>TenantId</c> ensuite.
/// </summary>
[ApiController]
[Route("api/endclients")]
public sealed class EndClientsController : ControllerBase
{
    // TODO: résoudre le tenant depuis le JWT Clerk (claim) au lieu d'une constante.
    private const string TenantId = "tenant_mvogo";

    private readonly IEndClientService _service;

    public EndClientsController(IEndClientService service)
    {
        _service = service;
    }

    /// <summary>GET /api/endclients?search= — liste paginable des clients du cabinet.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EndClient>>> List(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var items = await _service.ListAsync(TenantId, search, cancellationToken);
        return Ok(new { items, total = items.Count });
    }

    /// <summary>GET /api/endclients/{id} — détail d'un client.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EndClient>> Get(string id, CancellationToken cancellationToken)
    {
        var client = await _service.GetAsync(TenantId, id, cancellationToken);
        return client is null ? NotFound(new { error = "Client introuvable" }) : Ok(client);
    }

    /// <summary>POST /api/endclients — crée un client du cabinet.</summary>
    [HttpPost]
    public async Task<ActionResult<EndClient>> Create(
        [FromBody] CreateEndClientRequest request,
        CancellationToken cancellationToken)
    {
        var client = new EndClient
        {
            Id = $"client_{Guid.NewGuid():N}",
            TenantId = TenantId,
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            WhatsappNumber = request.WhatsappNumber,
            Email = request.Email,
            Siret = request.Siret,
            VatNumber = request.VatNumber,
            Tags = request.Tags ?? [],
            ActiveMonth = string.IsNullOrWhiteSpace(request.ActiveMonth)
                ? DateTimeOffset.UtcNow.ToString("yyyy-MM")
                : request.ActiveMonth,
            CreatedAt = DateTimeOffset.UtcNow
        };

        try
        {
            var created = await _service.CreateAsync(client, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>PUT /api/endclients/{id} — met à jour un client existant.</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EndClient>> Update(
        string id,
        [FromBody] UpdateEndClientRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await _service.GetAsync(TenantId, id, cancellationToken);
        if (existing is null)
        {
            return NotFound(new { error = "Client introuvable" });
        }

        existing.CompanyName = request.CompanyName;
        existing.ContactName = request.ContactName;
        existing.WhatsappNumber = request.WhatsappNumber;
        existing.Email = request.Email;
        existing.Siret = request.Siret;
        existing.VatNumber = request.VatNumber;
        existing.Tags = request.Tags ?? [];
        if (!string.IsNullOrWhiteSpace(request.ActiveMonth))
        {
            existing.ActiveMonth = request.ActiveMonth;
        }

        try
        {
            var updated = await _service.UpdateAsync(existing, cancellationToken);
            return updated is null ? NotFound(new { error = "Client introuvable" }) : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>DELETE /api/endclients/{id} — supprime un client.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(TenantId, id, cancellationToken);
        return deleted ? NoContent() : NotFound(new { error = "Client introuvable" });
    }
}
