using System.Text.Json.Serialization;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Contracts;

/// <summary>
/// DTO de désérialisation (partielle) d'un évènement webhook Stripe (spec §6.1).
/// On ne lit que ce qui pilote le cycle de vie de l'abonnement.
/// </summary>
public sealed class StripeWebhookPayload
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public StripeData? Data { get; set; }

    /// <summary>Projette le payload « fil » vers l'évènement neutre du domaine.</summary>
    public StripeEvent ToDomain() => new()
    {
        Type = Type ?? string.Empty,
        CustomerId = Data?.Object?.Customer,
        SubscriptionStatus = Data?.Object?.Status
    };

    public sealed class StripeData
    {
        [JsonPropertyName("object")]
        public StripeObject? Object { get; set; }
    }

    public sealed class StripeObject
    {
        [JsonPropertyName("customer")]
        public string? Customer { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}

/// <summary>Requête d'ouverture du portail client Stripe (spec §5.8).</summary>
public sealed class CreatePortalRequest
{
    /// <summary>URL de retour après passage dans le portail.</summary>
    public string? ReturnUrl { get; set; }
}
