using System.Text;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>
/// Implémentation BLL pour EndClients.
/// </summary>
public sealed class EndClientService : IEndClientService
{
    private readonly IEndClientRepository _repository;

    public EndClientService(IEndClientRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<EndClient>> ListAsync(
        string tenantId,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return _repository.ListAsync(tenantId, normalizedSearch, cancellationToken);
    }

    public Task<EndClient?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(tenantId, id, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(tenantId, id, cancellationToken);
    }

    public async Task<EndClient> CreateAsync(
        EndClient client,
        CancellationToken cancellationToken = default)
    {
        Validate(client);
        return await _repository.CreateAsync(client, cancellationToken);
    }

    public async Task<EndClient?> UpdateAsync(
        EndClient client,
        CancellationToken cancellationToken = default)
    {
        Validate(client);
        return await _repository.UpdateAsync(client, cancellationToken);
    }

    public async Task<ImportCsvResult> ImportCsvAsync(
        string tenantId,
        string csvContent,
        CancellationToken cancellationToken = default)
    {
        var result = new ImportCsvResult();

        if (string.IsNullOrWhiteSpace(csvContent))
        {
            result.Errors.Add(new ImportCsvError { Line = 0, Message = "CSV vide." });
            return result;
        }

        var rawLines = csvContent
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Split('\n');

        var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var headerParsed = false;

        for (var i = 0; i < rawLines.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lineNumber = i + 1;
            var line = rawLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);

            if (!headerParsed)
            {
                for (var c = 0; c < fields.Count; c++)
                {
                    columns[fields[c].Trim()] = c;
                }

                if (!columns.ContainsKey("companyName") ||
                    !columns.ContainsKey("contactName") ||
                    !columns.ContainsKey("whatsappNumber"))
                {
                    result.Errors.Add(new ImportCsvError
                    {
                        Line = lineNumber,
                        Message = "En-tête invalide : colonnes requises companyName, contactName, whatsappNumber."
                    });
                    return result;
                }

                headerParsed = true;
                continue;
            }

            string Field(string name) =>
                columns.TryGetValue(name, out var idx) && idx < fields.Count
                    ? fields[idx].Trim()
                    : string.Empty;

            var tagsRaw = Field("tags");
            var tags = string.IsNullOrWhiteSpace(tagsRaw)
                ? []
                : tagsRaw
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

            var activeMonth = Field("activeMonth");
            if (string.IsNullOrWhiteSpace(activeMonth))
            {
                activeMonth = DateTimeOffset.UtcNow.ToString("yyyy-MM");
            }

            var client = new EndClient
            {
                Id = $"client_{Guid.NewGuid():N}",
                TenantId = tenantId,
                CompanyName = Field("companyName"),
                ContactName = Field("contactName"),
                WhatsappNumber = Field("whatsappNumber"),
                Email = EmptyToNull(Field("email")),
                Siret = EmptyToNull(Field("siret")),
                VatNumber = EmptyToNull(Field("vatNumber")),
                Tags = tags,
                ActiveMonth = activeMonth,
                CreatedAt = DateTimeOffset.UtcNow
            };

            try
            {
                var created = await CreateAsync(client, cancellationToken);
                result.Clients.Add(created);
            }
            catch (ArgumentException ex)
            {
                result.Errors.Add(new ImportCsvError { Line = lineNumber, Message = ex.Message });
            }
        }

        if (!headerParsed)
        {
            result.Errors.Add(new ImportCsvError { Line = 0, Message = "Aucune ligne exploitable." });
        }

        return result;
    }

    private static string? EmptyToNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// Parse une ligne CSV : virgule = séparateur, guillemets doubles pour les
    /// champs contenant des virgules, <c>""</c> = guillemet échappé.
    /// </summary>
    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var sb = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }
            else
            {
                switch (ch)
                {
                    case '"':
                        inQuotes = true;
                        break;
                    case ',':
                        fields.Add(sb.ToString());
                        sb.Clear();
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
        }

        fields.Add(sb.ToString());
        return fields;
    }

    private static void Validate(EndClient client)
    {
        if (string.IsNullOrWhiteSpace(client.TenantId))
        {
            throw new ArgumentException("tenantId requis", nameof(client));
        }

        if (string.IsNullOrWhiteSpace(client.Id))
        {
            throw new ArgumentException("id requis", nameof(client));
        }

        if (string.IsNullOrWhiteSpace(client.CompanyName))
        {
            throw new ArgumentException("companyName requis", nameof(client));
        }

        if (string.IsNullOrWhiteSpace(client.ContactName))
        {
            throw new ArgumentException("contactName requis", nameof(client));
        }

        if (string.IsNullOrWhiteSpace(client.WhatsappNumber))
        {
            throw new ArgumentException("whatsappNumber requis", nameof(client));
        }
    }
}
