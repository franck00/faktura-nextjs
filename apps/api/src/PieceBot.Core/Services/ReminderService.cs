using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>Implémentation BLL des rappels mensuels (spec §5.6 / §6.7).</summary>
public sealed class ReminderService : IReminderService
{
    private readonly ITenantRepository _tenants;
    private readonly IEndClientRepository _clients;
    private readonly IPieceRepository _pieces;
    private readonly IMonthlyReminderRepository _reminders;

    public ReminderService(
        ITenantRepository tenants,
        IEndClientRepository clients,
        IPieceRepository pieces,
        IMonthlyReminderRepository reminders)
    {
        _tenants = tenants;
        _clients = clients;
        _pieces = pieces;
        _reminders = reminders;
    }

    public async Task<ReminderConfig?> GetConfigAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _tenants.GetAsync(tenantId, cancellationToken);
        return tenant is null ? null : ToConfig(tenant.Settings);
    }

    public async Task<ReminderConfig?> UpdateConfigAsync(
        string tenantId,
        ReminderConfig config,
        CancellationToken cancellationToken = default)
    {
        Validate(config);

        var tenant = await _tenants.GetAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        tenant.Settings.MonthlyReminderDay = config.MonthlyReminderDay;
        tenant.Settings.MonthlyReminderHour = config.MonthlyReminderHour;
        tenant.Settings.RemindersEnabled = config.Enabled;
        if (!string.IsNullOrWhiteSpace(config.MessageTemplate))
        {
            tenant.Settings.ReminderMessageTemplate = config.MessageTemplate;
        }

        var updated = await _tenants.UpdateAsync(tenant, cancellationToken);
        return updated is null ? null : ToConfig(updated.Settings);
    }

    public async Task<ReminderRunResult?> SendNowAsync(
        string tenantId,
        string? month = null,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _tenants.GetAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        var resolvedMonth = string.IsNullOrWhiteSpace(month)
            ? DateTimeOffset.UtcNow.ToString("yyyy-MM")
            : month;

        var result = new ReminderRunResult { Month = resolvedMonth };

        if (!tenant.Settings.RemindersEnabled)
        {
            return new ReminderRunResult { Month = resolvedMonth, Disabled = true };
        }

        var clients = await _clients.ListAsync(tenantId, cancellationToken: cancellationToken);

        foreach (var client in clients)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (client.RemindersDisabled)
            {
                result.Skipped++;
                continue;
            }

            var received = await _pieces.ListAsync(
                tenantId, resolvedMonth, client.Id, cancellationToken: cancellationToken);
            var validated = received.Count(p => p.Status == PieceStatus.Validated);
            var completionRate = received.Count == 0 ? 0d : (double)validated / received.Count;

            var existing = await _reminders.GetAsync(
                tenantId, client.Id, resolvedMonth, cancellationToken);

            var reminder = existing ?? new MonthlyReminder
            {
                Id = $"reminder_{Guid.NewGuid():N}",
                TenantId = tenantId,
                EndClientId = client.Id,
                Month = resolvedMonth,
                RemindersSent = 0
            };

            reminder.RemindersSent += 1;
            reminder.SentAt = DateTimeOffset.UtcNow;
            reminder.CompletionRate = completionRate;

            // TODO: envoyer le message WhatsApp (template tenant.Settings.ReminderMessageTemplate)
            // via l'infrastructure Meta Cloud API quand elle sera branchée.

            var saved = await _reminders.UpsertAsync(reminder, cancellationToken);
            result.Reminders.Add(saved);
        }

        return result;
    }

    private static ReminderConfig ToConfig(TenantSettings settings) => new()
    {
        MonthlyReminderDay = settings.MonthlyReminderDay,
        MonthlyReminderHour = settings.MonthlyReminderHour,
        Enabled = settings.RemindersEnabled,
        MessageTemplate = settings.ReminderMessageTemplate
    };

    private static void Validate(ReminderConfig config)
    {
        if (config.MonthlyReminderDay is < 1 or > 28)
        {
            throw new ArgumentException(
                "monthlyReminderDay doit être entre 1 et 28", nameof(config));
        }

        if (config.MonthlyReminderHour is < 0 or > 23)
        {
            throw new ArgumentException(
                "monthlyReminderHour doit être entre 0 et 23", nameof(config));
        }
    }
}
