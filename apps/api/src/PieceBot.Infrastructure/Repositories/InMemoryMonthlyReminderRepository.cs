using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Repositories;

/// <summary>
/// Implémentation mémoire provisoire de <see cref="IMonthlyReminderRepository"/>.
/// Démarre vide (les traces sont créées au fil des envois). À remplacer par un
/// repository Cosmos DB (partition tenantId) sans changer l'interface.
/// </summary>
public sealed class InMemoryMonthlyReminderRepository : IMonthlyReminderRepository
{
    // Clé composite : tenantId|endClientId|month.
    private readonly ConcurrentDictionary<string, MonthlyReminder> _reminders = new();

    private static string Key(string tenantId, string endClientId, string month) =>
        $"{tenantId}|{endClientId}|{month}";

    public Task<IReadOnlyList<MonthlyReminder>> ListAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MonthlyReminder> result = _reminders.Values
            .Where(r => r.TenantId == tenantId && r.Month == month)
            .OrderBy(r => r.EndClientId, StringComparer.Ordinal)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<MonthlyReminder?> GetAsync(
        string tenantId,
        string endClientId,
        string month,
        CancellationToken cancellationToken = default)
    {
        _reminders.TryGetValue(Key(tenantId, endClientId, month), out var reminder);
        return Task.FromResult(reminder);
    }

    public Task<MonthlyReminder> UpsertAsync(
        MonthlyReminder reminder,
        CancellationToken cancellationToken = default)
    {
        var key = Key(reminder.TenantId, reminder.EndClientId, reminder.Month);
        _reminders[key] = reminder;
        return Task.FromResult(reminder);
    }
}
