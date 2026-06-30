using PieceBot.Core.Domain;

namespace PieceBot.Core.Abstractions;

/// <summary>
/// Couche BLL pour les pièces justificatives (spec §6.4). L'API consomme ce
/// service ; il orchestre les règles métier (validation, correction) et délègue
/// la persistance au <see cref="IPieceRepository"/>.
/// </summary>
public interface IPieceService
{
    Task<IReadOnlyList<Piece>> ListAsync(
        string tenantId,
        string? month = null,
        string? endClientId = null,
        PieceStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<Piece?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>Valide l'extraction d'une pièce (passe en <see cref="PieceStatus.Validated"/>).</summary>
    Task<Piece?> ValidateAsync(
        string tenantId,
        string id,
        string validatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>Corrige la catégorie et/ou les données extraites d'une pièce.</summary>
    Task<Piece?> CorrectAsync(
        string tenantId,
        string id,
        PieceCategory? category,
        ExtractedData? extractedData,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default);
}
