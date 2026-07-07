using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;

namespace PieceBot.Infrastructure.Messaging;

/// <summary>
/// Implémentation provisoire de <see cref="IWhatsAppSender"/> : n'appelle pas
/// encore la Meta Cloud API. Conserve les derniers messages sortants en mémoire
/// (utile en dev / debug). À remplacer par l'appel HTTP réel + template.
/// </summary>
public sealed class StubWhatsAppSender : IWhatsAppSender
{
    /// <summary>Messages sortants récents (borné), pour inspection en dev.</summary>
    public sealed record OutboundMessage(string PhoneNumberId, string ToNumber, string Message, DateTimeOffset SentAt);

    private readonly ConcurrentQueue<OutboundMessage> _sent = new();

    public IReadOnlyCollection<OutboundMessage> Sent => _sent;

    public Task SendTextAsync(
        string phoneNumberId,
        string toNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        _sent.Enqueue(new OutboundMessage(phoneNumberId, toNumber, message, DateTimeOffset.UtcNow));

        // Borne mémoire : ne garde que les 100 derniers.
        while (_sent.Count > 100 && _sent.TryDequeue(out _))
        {
        }

        return Task.CompletedTask;
    }
}
