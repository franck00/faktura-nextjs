using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>
/// Implémentation BLL des statistiques dashboard. Reproduit côté serveur les
/// sélecteurs du frontend (getDashboardStats / getClientCompletions).
/// </summary>
public sealed class StatsService : IStatsService
{
    private readonly IPieceRepository _pieces;
    private readonly IEndClientRepository _clients;

    public StatsService(IPieceRepository pieces, IEndClientRepository clients)
    {
        _pieces = pieces;
        _clients = clients;
    }

    public async Task<DashboardStats> GetDashboardAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default)
    {
        var pieces = await _pieces.ListAsync(tenantId, month, cancellationToken: cancellationToken);

        var validated = pieces.Count(p => p.Status == PieceStatus.Validated);
        var pending = pieces.Count(p =>
            p.Status is PieceStatus.Extracted or PieceStatus.Received or PieceStatus.Processing);
        var activeClients = pieces.Select(p => p.EndClientId).Distinct().Count();

        return new DashboardStats
        {
            Month = month,
            TotalPieces = pieces.Count,
            PendingValidation = pending,
            ValidatedPieces = validated,
            ActiveClients = activeClients,
            CompletionRate = pieces.Count == 0 ? 0 : (double)validated / pieces.Count
        };
    }

    public async Task<IReadOnlyList<ClientCompletion>> GetCompletionAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default)
    {
        var clients = await _clients.ListAsync(tenantId, cancellationToken: cancellationToken);
        var pieces = await _pieces.ListAsync(tenantId, month, cancellationToken: cancellationToken);

        return clients
            .Select(c =>
            {
                var received = pieces.Where(p => p.EndClientId == c.Id).ToList();
                var validated = received.Count(p => p.Status == PieceStatus.Validated);
                return new ClientCompletion
                {
                    EndClientId = c.Id,
                    CompanyName = c.CompanyName,
                    PiecesReceived = received.Count,
                    PiecesValidated = validated,
                    LastPieceReceived = c.LastPieceReceived,
                    CompletionRate = received.Count == 0 ? 0 : (double)validated / received.Count
                };
            })
            .ToList();
    }
}
