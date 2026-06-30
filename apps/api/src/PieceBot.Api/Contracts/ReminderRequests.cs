namespace PieceBot.Api.Contracts;

/// <summary>Payload de mise à jour de la config rappels (PUT /api/reminders/config).</summary>
public sealed record UpdateReminderConfigRequest(
    int MonthlyReminderDay,
    int MonthlyReminderHour,
    bool Enabled,
    string? MessageTemplate);

/// <summary>Payload optionnel du déclenchement manuel (POST /api/reminders/send-now).</summary>
public sealed record SendRemindersRequest(string? Month);
