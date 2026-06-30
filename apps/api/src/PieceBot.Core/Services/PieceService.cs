using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>Implémentation BLL pour les pièces justificatives.</summary>
public sealed class PieceService : IPieceService
{
    private readonly IPieceRepository _repository;

    public PieceService(IPieceRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Piece>> ListAsync(
        string tenantId,
        string? month = null,
        string? endClientId = null,
        PieceStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        return _repository.ListAsync(tenantId, month, endClientId, status, cancellationToken);
    }

    public Task<Piece?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(tenantId, id, cancellationToken);
    }

    public async Task<Piece?> ValidateAsync(
        string tenantId,
        string id,
        string validatedBy,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(validatedBy))
        {
            throw new ArgumentException("validatedBy requis", nameof(validatedBy));
        }

        var piece = await _repository.GetAsync(tenantId, id, cancellationToken);
        if (piece is null)
        {
            return null;
        }

        piece.Status = PieceStatus.Validated;
        piece.ValidatedBy = validatedBy;
        piece.ValidatedAt = DateTimeOffset.UtcNow;

        return await _repository.UpdateAsync(piece, cancellationToken);
    }

    public async Task<Piece?> CorrectAsync(
        string tenantId,
        string id,
        PieceCategory? category,
        ExtractedData? extractedData,
        CancellationToken cancellationToken = default)
    {
        var piece = await _repository.GetAsync(tenantId, id, cancellationToken);
        if (piece is null)
        {
            return null;
        }

        if (category is not null)
        {
            piece.Category = category.Value;
        }

        if (extractedData is not null)
        {
            piece.ExtractedData = extractedData;
        }

        return await _repository.UpdateAsync(piece, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(tenantId, id, cancellationToken);
    }
}
