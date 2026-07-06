namespace PieceBot.Core.Domain;

/// <summary>
/// Trace d'un rappel mensuel envoyé à un client (collection Cosmos
/// <c>monthlyReminders</c>, spec §4.5). Partitionnée par <see cref="TenantId"/>.
/// Un seul document par (tenant, client, mois) ; <see cref="RemindersSent"/>
/// s'incrémente à chaque relance (J+7, J+14…).
/// </summary>
public sealed class MonthlyReminder
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public string Type { get; init; } = "reminder";

    public required string EndClientId { get; init; }

    /// <summary>Mois concerné, format <c>YYYY-MM</c>.</summary>
    public required string Month { get; init; }

    public DateTimeOffset SentAt { get; set; }
    public int RemindersSent { get; set; }

    /// <summary>Taux de complétion du client au moment du rappel, 0..1.</summary>
    public double CompletionRate { get; set; }
}

/// <summary>
/// Configuration des rappels d'un cabinet (spec §6.7). Reflète les
/// <see cref="TenantSettings"/> liées aux rappels.
/// </summary>
public sealed class ReminderConfig
{
    /// <summary>Jour du mois d'envoi du rappel initial (1..28).</summary>
    public int MonthlyReminderDay { get; set; }

    /// <summary>Heure d'envoi (0..23), dans le fuseau du cabinet.</summary>
    public int MonthlyReminderHour { get; set; }

    public bool Enabled { get; set; }

    public string MessageTemplate { get; set; } = string.Empty;
}

/// <summary>Résultat d'un déclenchement de rappels (spec §6.7 — send-now / cron).</summary>
public sealed class ReminderRunResult
{
    public required string Month { get; init; }

    /// <summary>Vrai si les rappels sont désactivés au niveau du cabinet.</summary>
    public bool Disabled { get; init; }

    public List<MonthlyReminder> Reminders { get; } = [];

    /// <summary>Clients ignorés (rappels désactivés pour le client).</summary>
    public int Skipped { get; set; }

    /// <summary>Nombre de rappels envoyés (créés ou relancés).</summary>
    public int Sent => Reminders.Count;
}
