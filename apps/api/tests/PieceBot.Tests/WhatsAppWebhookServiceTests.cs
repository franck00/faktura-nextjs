using System.Security.Cryptography;
using System.Text;
using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Messaging;
using PieceBot.Infrastructure.Ocr;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class WhatsAppWebhookServiceTests
{
    // DemoData : tenant_mvogo, Phone Number ID "1234567890", client_aissa +237699123456.
    private const string PhoneNumberId = "1234567890";
    private const string AissaNumber = "237699123456"; // format Meta (sans +)

    private static (WhatsAppWebhookService Service, StubWhatsAppSender Sender, InMemoryPieceRepository Pieces) BuildService()
    {
        var sender = new StubWhatsAppSender();
        var pieces = new InMemoryPieceRepository();
        var service = new WhatsAppWebhookService(
            new InMemoryTenantRepository(),
            new InMemoryEndClientRepository(),
            pieces,
            new StubWhatsAppMediaStore(),
            new InMemoryMediaStore(),
            new NoOpReceiptExtractor(),
            sender);
        return (service, sender, pieces);
    }

    private static WhatsAppWebhookContext Context() =>
        new() { PhoneNumberId = PhoneNumberId };

    // ── Vérification (GET handshake) ────────────────────────────────────────

    [Fact]
    public void HandleVerification_ReturnsChallenge_WhenTokenMatches()
    {
        var (service, _, _) = BuildService();

        var result = service.HandleVerification("subscribe", "secret-token", "chal-123", "secret-token");

        Assert.Equal("chal-123", result);
    }

    [Fact]
    public void HandleVerification_ReturnsNull_WhenTokenMismatch()
    {
        var (service, _, _) = BuildService();

        var result = service.HandleVerification("subscribe", "wrong", "chal-123", "secret-token");

        Assert.Null(result);
    }

    // ── Signature HMAC (POST) ───────────────────────────────────────────────

    [Fact]
    public void VerifySignature_TrueForValidSignature()
    {
        var (service, _, _) = BuildService();
        const string appSecret = "app-secret";
        var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
        var expected = "sha256=" + Convert.ToHexStringLower(
            new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)).ComputeHash(body));

        Assert.True(service.VerifySignature(body, expected, appSecret));
    }

    [Fact]
    public void VerifySignature_FalseForTamperedBody()
    {
        var (service, _, _) = BuildService();
        const string appSecret = "app-secret";
        var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
        var signature = "sha256=" + Convert.ToHexStringLower(
            new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)).ComputeHash(body));
        var tampered = Encoding.UTF8.GetBytes("{\"hello\":\"tampered\"}");

        Assert.False(service.VerifySignature(tampered, signature, appSecret));
    }

    [Fact]
    public void VerifySignature_FalseForMissingHeader()
    {
        var (service, _, _) = BuildService();

        Assert.False(service.VerifySignature([1, 2, 3], null, "app-secret"));
    }

    // ── Traitement des messages ─────────────────────────────────────────────

    [Fact]
    public async Task ProcessAsync_CreatesPiece_ForImageFromKnownClient()
    {
        var (service, sender, pieces) = BuildService();
        var message = new WhatsAppInboundMessage
        {
            From = AissaNumber,
            MessageId = "wamid.test1",
            Timestamp = DateTimeOffset.Parse("2026-06-29T10:00:00Z"),
            Kind = WhatsAppMessageKind.Image,
            MediaId = "media_123",
            MimeType = "image/jpeg"
        };

        var result = await service.ProcessAsync(Context(), message);

        Assert.Equal(WebhookOutcome.PieceCreated, result.Outcome);
        Assert.NotNull(result.PieceId);

        var stored = await pieces.GetAsync("tenant_mvogo", result.PieceId!);
        Assert.NotNull(stored);
        Assert.Equal("client_aissa", stored!.EndClientId);
        Assert.Equal(PieceStatus.Received, stored.Status);
        Assert.Equal("2026-06", stored.Month);
        Assert.Equal("wamid.test1", stored.WhatsappMessageId);

        // Un accusé de réception a été envoyé au client.
        Assert.Single(sender.Sent);
        Assert.Equal(AissaNumber, sender.Sent.First().ToNumber);
    }

    [Fact]
    public async Task ProcessAsync_GreetsUnknownSender_WithoutCreatingPiece()
    {
        var (service, sender, pieces) = BuildService();
        var message = new WhatsAppInboundMessage
        {
            From = "237600000000", // numéro inconnu
            MessageId = "wamid.test2",
            Timestamp = DateTimeOffset.UtcNow,
            Kind = WhatsAppMessageKind.Image,
            MediaId = "media_x",
            MimeType = "image/jpeg"
        };

        var result = await service.ProcessAsync(Context(), message);

        Assert.Equal(WebhookOutcome.UnknownSender, result.Outcome);
        Assert.Null(result.PieceId);
        Assert.Single(sender.Sent);
        Assert.Contains("Cabinet Mvogo", sender.Sent.First().Message);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsTenantNotFound_ForUnknownPhoneNumberId()
    {
        var (service, _, _) = BuildService();
        var context = new WhatsAppWebhookContext { PhoneNumberId = "0000000000" };
        var message = new WhatsAppInboundMessage
        {
            From = AissaNumber,
            MessageId = "wamid.test3",
            Timestamp = DateTimeOffset.UtcNow,
            Kind = WhatsAppMessageKind.Image,
            MediaId = "media_y",
            MimeType = "image/jpeg"
        };

        var result = await service.ProcessAsync(context, message);

        Assert.Equal(WebhookOutcome.TenantNotFound, result.Outcome);
    }

    [Fact]
    public async Task ProcessAsync_AcknowledgesTextMessage_WithoutCreatingPiece()
    {
        var (service, sender, _) = BuildService();
        var message = new WhatsAppInboundMessage
        {
            From = AissaNumber,
            MessageId = "wamid.test4",
            Timestamp = DateTimeOffset.UtcNow,
            Kind = WhatsAppMessageKind.Text,
            Text = "Bonjour"
        };

        var result = await service.ProcessAsync(Context(), message);

        Assert.Equal(WebhookOutcome.Acknowledged, result.Outcome);
        Assert.Null(result.PieceId);
        Assert.Single(sender.Sent);
    }
}
