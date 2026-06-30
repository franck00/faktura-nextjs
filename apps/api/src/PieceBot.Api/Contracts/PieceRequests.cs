using PieceBot.Core.Domain;

namespace PieceBot.Api.Contracts;

/// <summary>Payload de validation d'une pièce (PUT /api/pieces/{id}/validate).</summary>
public sealed record ValidatePieceRequest(string? ValidatedBy);

/// <summary>Payload de correction d'extraction (PUT /api/pieces/{id}).</summary>
public sealed record CorrectPieceRequest(
    PieceCategory? Category,
    ExtractedData? ExtractedData);
