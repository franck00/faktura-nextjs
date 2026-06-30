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
