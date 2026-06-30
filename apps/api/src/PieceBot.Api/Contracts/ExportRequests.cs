namespace PieceBot.Api.Contracts;

/// <summary>
/// Payload de création d'un export mensuel (POST /api/exports/monthly-pdf|excel).
/// <c>EndClientId</c> null = export par lot (tous les clients du mois).
/// </summary>
public sealed record CreateExportRequest(
    string Month,
    string? EndClientId);
