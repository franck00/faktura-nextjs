using Microsoft.AspNetCore.Mvc;

namespace PieceBot.Api.Auth;

/// <summary>
/// Résolution du tenant courant à partir du JWT Clerk (spec §2.3). En attendant
/// que Clerk soit configuré, on retombe sur un tenant de démo constant.
/// </summary>
public static class TenantResolution
{
    /// <summary>Tenant de secours tant que l'auth Clerk n'est pas active.</summary>
    public const string FallbackTenantId = "tenant_mvogo";

    /// <summary>Utilisateur de secours (validateur) tant que l'auth n'est pas active.</summary>
    public const string FallbackUserId = "clerk_user_id";

    /// <summary>
    /// Lit l'identifiant de cabinet depuis les claims du token (Clerk : claim
    /// personnalisé <c>tenantId</c> via un JWT template, ou <c>org_id</c> si
    /// l'on utilise les Organizations), sinon renvoie le fallback.
    /// </summary>
    public static string GetTenantId(this HttpContext context) =>
        context.User.FindFirst("tenantId")?.Value
        ?? context.User.FindFirst("org_id")?.Value
        ?? FallbackTenantId;

    /// <summary>Identifiant de l'utilisateur Clerk (claim <c>sub</c>), sinon fallback.</summary>
    public static string GetUserId(this HttpContext context) =>
        context.User.FindFirst("sub")?.Value ?? FallbackUserId;
}

/// <summary>
/// Base des controllers scopés cabinet : expose <see cref="TenantId"/> et
/// <see cref="CurrentUserId"/> résolus depuis le JWT (ou fallback démo).
/// </summary>
public abstract class TenantControllerBase : ControllerBase
{
    protected string TenantId => HttpContext.GetTenantId();
    protected string CurrentUserId => HttpContext.GetUserId();
}
