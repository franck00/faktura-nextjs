using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Repositories;

/// <summary>
/// Implémentation mémoire provisoire de <see cref="IEndClientRepository"/>.
/// Seedée avec des données de démo. À remplacer par un repository Cosmos DB
/// (partition tenantId) sans changer l'interface.
/// </summary>
public sealed class InMemoryEndClientRepository : IEndClientRepository
{
    private readonly ConcurrentDictionary<string, EndClient> _clients = new();

    public InMemoryEndClientRepository()
    {
        foreach (var client in DemoData.EndClients())
        {
            _clients[client.Id] = client;
        }
    }

    public Task<IReadOnlyList<EndClient>> ListAsync(
        string tenantId,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<EndClient> query = _clients.Values.Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            query = query.Where(c =>
                c.CompanyName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.ContactName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.WhatsappNumber.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.Tags.Any(t => t.Contains(q, StringComparison.OrdinalIgnoreCase)));
        }

        IReadOnlyList<EndClient> result = query
            .OrderBy(c => c.CompanyName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<EndClient?> GetAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        _clients.TryGetValue(id, out var client);
        return Task.FromResult(client is not null && client.TenantId == tenantId ? client : null);
    }

    public Task<EndClient> CreateAsync(EndClient client, CancellationToken cancellationToken = default)
    {
        _clients[client.Id] = client;
        return Task.FromResult(client);
    }

    public Task<EndClient?> UpdateAsync(EndClient client, CancellationToken cancellationToken = default)
    {
        if (!_clients.ContainsKey(client.Id))
        {
            return Task.FromResult<EndClient?>(null);
        }

        _clients[client.Id] = client;
        return Task.FromResult<EndClient?>(client);
    }

    public Task<bool> DeleteAsync(
        string tenantId,
        string id,
        CancellationToken cancellationToken = default)
    {
        if (_clients.TryGetValue(id, out var client) && client.TenantId == tenantId)
        {
            return Task.FromResult(_clients.TryRemove(id, out _));
        }

        return Task.FromResult(false);
    }
}
