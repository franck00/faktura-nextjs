using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Repositories;

/// <summary>
/// Implémentation mémoire provisoire de <see cref="ITenantRepository"/>, seedée
/// avec le cabinet de démo. À remplacer par un repository Cosmos DB (partition
/// tenantId) sans changer l'interface.
/// </summary>
public sealed class InMemoryTenantRepository : ITenantRepository
{
    private readonly ConcurrentDictionary<string, Tenant> _tenants = new();

    public InMemoryTenantRepository()
    {
        var tenant = DemoData.Tenant();
        _tenants[tenant.TenantId] = tenant;
    }

    public Task<Tenant?> GetAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _tenants.TryGetValue(tenantId, out var tenant);
        return Task.FromResult(tenant);
    }

    public Task<Tenant?> GetByWhatsappPhoneNumberIdAsync(
        string phoneNumberId,
        CancellationToken cancellationToken = default)
    {
        var tenant = _tenants.Values.FirstOrDefault(t => t.WhatsappPhoneNumberId == phoneNumberId);
        return Task.FromResult(tenant);
    }

    public Task<Tenant?> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        if (!_tenants.ContainsKey(tenant.TenantId))
        {
            return Task.FromResult<Tenant?>(null);
        }

        _tenants[tenant.TenantId] = tenant;
        return Task.FromResult<Tenant?>(tenant);
    }
}
