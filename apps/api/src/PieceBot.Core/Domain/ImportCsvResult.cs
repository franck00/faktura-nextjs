namespace PieceBot.Core.Domain;

/// <summary>Erreur d'import sur une ligne CSV donnée (1-based, en-tête inclus).</summary>
public sealed class ImportCsvError
{
    public int Line { get; init; }
    public required string Message { get; init; }
}

/// <summary>
/// Résultat d'un import CSV de clients (spec §6.3 — <c>POST /api/endclients/import-csv</c>).
/// </summary>
public sealed class ImportCsvResult
{
    public List<EndClient> Clients { get; } = [];
    public List<ImportCsvError> Errors { get; } = [];

    /// <summary>Nombre de clients créés.</summary>
    public int Created => Clients.Count;

    /// <summary>Nombre de lignes ignorées (en erreur).</summary>
    public int Skipped => Errors.Count;
}
