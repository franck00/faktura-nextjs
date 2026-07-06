namespace PieceBot.Core.Abstractions;

/// <summary>
/// Port d'envoi de messages WhatsApp sortants (accusés de réception, spec §5.3).
/// L'implémentation réelle appelle la Meta Cloud API ; un stub loggue en attendant.
/// </summary>
public interface IWhatsAppSender
{
    Task SendTextAsync(
        string phoneNumberId,
        string toNumber,
        string message,
        CancellationToken cancellationToken = default);
}
