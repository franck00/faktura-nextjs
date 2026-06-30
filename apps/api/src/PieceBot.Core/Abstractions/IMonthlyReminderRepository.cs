using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Accès aux traces de rappels mensuels (collection Cosmos <c>monthlyReminders</c>,
/// spec §4.5 / §6.7). Toujours scopé par <paramref name="tenantId"/>.
/// </summary>
public interface IMonthlyReminderRepository
{
    Task<IReadOnlyList<MonthlyReminder>> ListAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default);

    Task<MonthlyReminder?> GetAsync(
        string tenantId,
        string endClientId,
        string month,
        CancellationToken cancellationToken = default);

    /// <summary>Crée ou met à jour la trace de rappel (un doc par tenant/client/mois).</summary>
    Task<MonthlyReminder> UpsertAsync(
        MonthlyReminder reminder,
        CancellationToken cancellationToken = default);
}
