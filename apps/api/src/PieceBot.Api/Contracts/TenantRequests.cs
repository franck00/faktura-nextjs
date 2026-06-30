namespace PieceBot.Api.Contracts;

/// <summary>Préférences modifiables du cabinet (PUT /api/tenants/me).</summary>
public sealed record UpdateTenantSettings(
    string Language,
    string Timezone,
    int MonthlyReminderDay,
    int MonthlyReminderHour);

/// <summary>Payload de mise à jour du cabinet courant (PUT /api/tenants/me).</summary>
public sealed record UpdateTenantRequest(
    string Name,
    string Country,
    string Currency,
    decimal VatRate,
    UpdateTenantSettings? Settings);

/// <summary>Payload de liaison du numéro WhatsApp (POST /api/tenants/me/whatsapp-link).</summary>
public sealed record LinkWhatsappRequest(
    string PhoneNumberId,
    string? BusinessAccountId);
