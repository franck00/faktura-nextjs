namespace PieceBot.Core.Domain;

/// <summary>Statut d'abonnement Stripe du cabinet (spec §4.1).</summary>
public enum SubscriptionStatus
{
    Trialing,
    Active,
    PastDue,
    Canceled
}

/// <summary>Plan d'abonnement (unique au démarrage — spec §1.3).</summary>
public enum SubscriptionPlan
{
    Standard
}

/// <summary>Propriétaire / utilisateur Clerk du cabinet.</summary>
public sealed class TenantOwner
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
}

/// <summary>Préférences du cabinet (langue, fuseau, rappels mensuels).</summary>
public sealed class TenantSettings
{
    public string Language { get; set; } = "fr";
    public string Timezone { get; set; } = "Africa/Douala";
    public int MonthlyReminderDay { get; set; } = 5;
    public int MonthlyReminderHour { get; set; } = 9;

    /// <summary>Active/désactive les rappels mensuels au niveau du cabinet (spec §5.6).</summary>
    public bool RemindersEnabled { get; set; } = true;

    /// <summary>
    /// Modèle de message de rappel, personnalisable par cabinet (spec §5.6).
    /// Placeholders supportés : <c>{client}</c> (nom de l'entreprise), <c>{month}</c>.
    /// </summary>
    public string ReminderMessageTemplate { get; set; } =
        "Bonjour {client}, merci de nous transmettre vos pièces justificatives du mois {month} via WhatsApp.";
}

/// <summary>
/// Cabinet comptable = locataire (collection Cosmos <c>tenants</c>, spec §4.1).
/// Partitionné par <see cref="TenantId"/>. Les codes pays/devise restent des
/// chaînes (ex. <c>CM</c>, <c>XAF</c>) pour matcher exactement le contrat frontend.
/// </summary>
public sealed class Tenant
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public string Type { get; init; } = "tenant";

    public required string Name { get; set; }

    /// <summary>Code pays ISO court : <c>CM</c>, <c>CI</c>, <c>SN</c>.</summary>
    public required string Country { get; set; }

    /// <summary>Devise : <c>XAF</c>, <c>XOF</c>, <c>EUR</c>.</summary>
    public required string Currency { get; set; }

    public decimal VatRate { get; set; }

    public string WhatsappPhoneNumberId { get; set; } = string.Empty;
    public string WhatsappBusinessAccountId { get; set; } = string.Empty;

    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trialing;
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Standard;
    public string? StripeCustomerId { get; set; }
    public DateTimeOffset? TrialEndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; init; }

    public required TenantOwner Owner { get; set; }
    public TenantSettings Settings { get; set; } = new();
}
