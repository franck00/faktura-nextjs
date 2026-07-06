namespace PieceBot.Core.Domain;

/// <summary>Issue du traitement d'un évènement webhook Stripe.</summary>
public enum BillingWebhookOutcome
{
    Ignored,
    TenantNotFound,
    SubscriptionUpdated
}

/// <summary>Résultat du traitement d'un évènement Stripe par la BLL.</summary>
public sealed class BillingWebhookResult
{
    public required BillingWebhookOutcome Outcome { get; init; }
    public string? TenantId { get; init; }
    public SubscriptionStatus? NewStatus { get; init; }

    public static BillingWebhookResult Ignored() =>
        new() { Outcome = BillingWebhookOutcome.Ignored };

    public static BillingWebhookResult TenantNotFound() =>
        new() { Outcome = BillingWebhookOutcome.TenantNotFound };
}

/// <summary>Évènement Stripe normalisé (indépendant du format « fil »).</summary>
public sealed class StripeEvent
{
    public required string Type { get; init; }

    /// <summary>Identifiant client Stripe rattaché à l'objet de l'évènement.</summary>
    public string? CustomerId { get; init; }

    /// <summary>Statut d'abonnement brut fourni par Stripe (ex. <c>active</c>).</summary>
    public string? SubscriptionStatus { get; init; }
}
