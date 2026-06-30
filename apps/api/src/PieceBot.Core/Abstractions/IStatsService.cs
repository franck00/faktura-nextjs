using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL des statistiques dashboard (spec §6.6). Agrège pièces + clients
/// pour un tenant et un mois donnés.
/// </summary>
public interface IStatsService
{
    Task<DashboardStats> GetDashboardAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClientCompletion>> GetCompletionAsync(
        string tenantId,
        string month,
        CancellationToken cancellationToken = default);
}
