using Microsoft.AspNetCore.Mvc;

namespace PieceBot.Api.Controllers;

/// <summary>Sonde de santé pour le monitoring / les checks de déploiement.</summary>
[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        service = "piecebot-api",
        timestamp = DateTimeOffset.UtcNow
    });
}
