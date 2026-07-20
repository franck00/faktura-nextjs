using Microsoft.AspNetCore.Mvc;
using PieceBot.Core.Abstractions;

namespace PieceBot.Api.Controllers;

/// <summary>
/// Sert les binaires de pièces téléchargés depuis WhatsApp (photo/PDF), rangés
/// par <see cref="IMediaStore"/> sous une clé <c>tenant/mois/media.ext</c>.
/// </summary>
[ApiController]
[Route("api/media")]
public sealed class MediaController : ControllerBase
{
    private readonly IMediaStore _store;

    public MediaController(IMediaStore store)
    {
        _store = store;
    }

    /// <summary>GET /api/media/{key} — renvoie le binaire (clé avec des « / »).</summary>
    [HttpGet("{**key}")]
    public async Task<IActionResult> Get(string key, CancellationToken cancellationToken)
    {
        var file = await _store.GetAsync(key, cancellationToken);
        return file is null
            ? NotFound(new { error = "Média introuvable" })
            : File(file.Bytes, file.ContentType, file.FileName);
    }
}
