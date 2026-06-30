namespace PieceBot.Core.Domain;

/// <summary>KPIs du dashboard pour un mois donné (spec §5.5 / §6.6).</summary>
public sealed class DashboardStats
{
    public required string Month { get; init; }
    public int TotalPieces { get; init; }
    public int PendingValidation { get; init; }
    public int ValidatedPieces { get; init; }
    public int ActiveClients { get; init; }

    /// <summary>Taux de complétion global du mois, 0..1.</summary>
    public double CompletionRate { get; init; }
}

/// <summary>Avancement d'un client du cabinet sur un mois donné (spec §6.6).</summary>
public sealed class ClientCompletion
{
    public required string EndClientId { get; init; }
    public required string CompanyName { get; init; }
    public int PiecesReceived { get; init; }
    public int PiecesValidated { get; init; }
    public DateTimeOffset? LastPieceReceived { get; init; }

    /// <summary>Part des pièces validées sur les pièces reçues, 0..1.</summary>
    public double CompletionRate { get; init; }
}
