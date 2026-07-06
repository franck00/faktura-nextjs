using System.Text.Json.Serialization;
using PieceBot.Core.Domain;

namespace PieceBot.Api.Contracts;

/// <summary>
/// DTO de désérialisation du payload webhook Meta WhatsApp (v18+, spec §3.1).
/// Reste dans la couche API (format « fil ») ; <see cref="ExtractMessages"/> le
/// projette vers les types neutres du domaine consommés par la BLL.
/// </summary>
public sealed class WhatsAppWebhookPayload
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("entry")]
    public List<Entry>? Entries { get; set; }

    /// <summary>Aplati le payload en couples (contexte, message) prêts pour la BLL.</summary>
    public IEnumerable<(WhatsAppWebhookContext Context, WhatsAppInboundMessage Message)> ExtractMessages()
    {
        if (Entries is null)
        {
            yield break;
        }

        foreach (var entry in Entries)
        {
            foreach (var change in entry.Changes ?? [])
            {
                var value = change.Value;
                if (value?.Messages is null || value.Metadata?.PhoneNumberId is null)
                {
                    continue;
                }

                var context = new WhatsAppWebhookContext
                {
                    PhoneNumberId = value.Metadata.PhoneNumberId,
                    ContactName = value.Contacts?.FirstOrDefault()?.Profile?.Name
                };

                foreach (var message in value.Messages)
                {
                    yield return (context, message.ToDomain());
                }
            }
        }
    }

    public sealed class Entry
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("changes")]
        public List<Change>? Changes { get; set; }
    }

    public sealed class Change
    {
        [JsonPropertyName("value")]
        public ChangeValue? Value { get; set; }

        [JsonPropertyName("field")]
        public string? Field { get; set; }
    }

    public sealed class ChangeValue
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }
    }

    public sealed class Metadata
    {
        [JsonPropertyName("display_phone_number")]
        public string? DisplayPhoneNumber { get; set; }

        [JsonPropertyName("phone_number_id")]
        public string? PhoneNumberId { get; set; }
    }

    public sealed class Contact
    {
        [JsonPropertyName("profile")]
        public Profile? Profile { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }
    }

    public sealed class Profile
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public sealed class Message
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public TextBody? Text { get; set; }

        [JsonPropertyName("image")]
        public MediaBody? Image { get; set; }

        [JsonPropertyName("document")]
        public MediaBody? Document { get; set; }

        [JsonPropertyName("audio")]
        public MediaBody? Audio { get; set; }

        [JsonPropertyName("video")]
        public MediaBody? Video { get; set; }

        public WhatsAppInboundMessage ToDomain()
        {
            var kind = Type switch
            {
                "text" => WhatsAppMessageKind.Text,
                "image" => WhatsAppMessageKind.Image,
                "document" => WhatsAppMessageKind.Document,
                "audio" => WhatsAppMessageKind.Audio,
                "video" => WhatsAppMessageKind.Video,
                _ => WhatsAppMessageKind.Unsupported
            };

            var media = Image ?? Document ?? Audio ?? Video;
            var timestamp = long.TryParse(Timestamp, out var unix)
                ? DateTimeOffset.FromUnixTimeSeconds(unix)
                : DateTimeOffset.UtcNow;

            return new WhatsAppInboundMessage
            {
                From = From ?? string.Empty,
                MessageId = Id ?? string.Empty,
                Timestamp = timestamp,
                Kind = kind,
                Text = Text?.Body,
                Caption = media?.Caption,
                MediaId = media?.Id,
                MimeType = media?.MimeType,
                FileName = Document?.Filename
            };
        }
    }

    public sealed class TextBody
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public sealed class MediaBody
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }
    }
}
