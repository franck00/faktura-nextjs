using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Core.Services;

/// <summary>Implémentation BLL du cabinet courant (spec §6.2).</summary>
public sealed class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public Task<Tenant?> GetAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(tenantId, cancellationToken);
    }

    public async Task<Tenant?> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        Validate(tenant);
        return await _repository.UpdateAsync(tenant, cancellationToken);
    }

    public async Task<Tenant?> LinkWhatsappAsync(
        string tenantId,
        string phoneNumberId,
        string businessAccountId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumberId))
        {
            throw new ArgumentException("phoneNumberId requis", nameof(phoneNumberId));
        }

        var tenant = await _repository.GetAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        tenant.WhatsappPhoneNumberId = phoneNumberId.Trim();
        tenant.WhatsappBusinessAccountId = businessAccountId?.Trim() ?? string.Empty;

        return await _repository.UpdateAsync(tenant, cancellationToken);
    }

    private static void Validate(Tenant tenant)
    {
        if (string.IsNullOrWhiteSpace(tenant.Name))
        {
            throw new ArgumentException("name requis", nameof(tenant));
        }

        if (string.IsNullOrWhiteSpace(tenant.Country))
        {
            throw new ArgumentException("country requis", nameof(tenant));
        }

        if (string.IsNullOrWhiteSpace(tenant.Currency))
        {
            throw new ArgumentException("currency requis", nameof(tenant));
        }

        if (tenant.VatRate < 0)
        {
            throw new ArgumentException("vatRate doit être >= 0", nameof(tenant));
        }
    }
}
