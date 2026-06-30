using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL des rappels mensuels (spec §6.7). La config vit dans les settings
/// du tenant ; <see cref="SendNowAsync"/> sera appelé par l'endpoint manuel et
/// par le cron Azure Function (spec §5.6). L'envoi WhatsApp réel sera branché
/// dans l'infrastructure.
/// </summary>
public interface IReminderService
{
    Task<ReminderConfig?> GetConfigAsync(
        string tenantId,
        CancellationToken cancellationToken = default);

    Task<ReminderConfig?> UpdateConfigAsync(
        string tenantId,
        ReminderConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Déclenche les rappels du mois pour tous les clients non désactivés.
    /// Renvoie <c>null</c> si le tenant est introuvable.
    /// </summary>
    Task<ReminderRunResult?> SendNowAsync(
        string tenantId,
        string? month = null,
        CancellationToken cancellationToken = default);
}
