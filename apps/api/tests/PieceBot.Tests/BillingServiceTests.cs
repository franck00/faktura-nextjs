using System.Security.Cryptography;
using System.Text;
using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Billing;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class BillingServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string CustomerId = "cus_demo123";

    /// <summary>Repo tenant seedé, avec un client Stripe rattaché au cabinet de démo.</summary>
    private static async Task<InMemoryTenantRepository> TenantsWithStripeCustomer()
    {
        var repo = new InMemoryTenantRepository();
        var tenant = await repo.GetAsync(TenantId);
        tenant!.StripeCustomerId = CustomerId;
        await repo.UpdateAsync(tenant);
        return repo;
    }

    private static BillingService Service(InMemoryTenantRepository tenants) =>
        new(tenants, new StubStripeGateway());

    private static string StripeSignature(byte[] body, string secret, long timestamp)
    {
        var signedPayload = Encoding.ASCII.GetBytes($"{timestamp}.")
            .Concat(body).ToArray();
        var sig = Convert.ToHexStringLower(
            new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(signedPayload));
        return $"t={timestamp},v1={sig}";
    }

    // ── Signature ───────────────────────────────────────────────────────────

    [Fact]
    public void VerifySignature_TrueForValidRecentSignature()
    {
        var service = Service(new InMemoryTenantRepository());
        const string secret = "whsec_test";
        var body = Encoding.UTF8.GetBytes("{\"id\":\"evt_1\"}");
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var header = StripeSignature(body, secret, now);

        Assert.True(service.VerifySignature(body, header, secret));
    }

    [Fact]
    public void VerifySignature_FalseForExpiredTimestamp()
    {
        var service = Service(new InMemoryTenantRepository());
        const string secret = "whsec_test";
        var body = Encoding.UTF8.GetBytes("{\"id\":\"evt_1\"}");
        var old = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();
        var header = StripeSignature(body, secret, old);

        Assert.False(service.VerifySignature(body, header, secret));
    }

    [Fact]
    public void VerifySignature_FalseForWrongSecret()
    {
        var service = Service(new InMemoryTenantRepository());
        var body = Encoding.UTF8.GetBytes("{\"id\":\"evt_1\"}");
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var header = StripeSignature(body, "whsec_test", now);

        Assert.False(service.VerifySignature(body, header, "whsec_other"));
    }

    // ── Cycle de vie de l'abonnement ────────────────────────────────────────

    [Fact]
    public async Task HandleEventAsync_ActivatesSubscription_OnCheckoutCompleted()
    {
        var tenants = await TenantsWithStripeCustomer();
        var service = Service(tenants);

        var result = await service.HandleEventAsync(new StripeEvent
        {
            Type = "checkout.session.completed",
            CustomerId = CustomerId
        });

        Assert.Equal(BillingWebhookOutcome.SubscriptionUpdated, result.Outcome);
        Assert.Equal(SubscriptionStatus.Active, result.NewStatus);
        var tenant = await tenants.GetAsync(TenantId);
        Assert.Equal(SubscriptionStatus.Active, tenant!.SubscriptionStatus);
    }

    [Fact]
    public async Task HandleEventAsync_CancelsSubscription_OnSubscriptionDeleted()
    {
        var tenants = await TenantsWithStripeCustomer();
        var service = Service(tenants);

        var result = await service.HandleEventAsync(new StripeEvent
        {
            Type = "customer.subscription.deleted",
            CustomerId = CustomerId
        });

        Assert.Equal(SubscriptionStatus.Canceled, result.NewStatus);
    }

    [Fact]
    public async Task HandleEventAsync_MapsPastDue_OnPaymentFailed()
    {
        var tenants = await TenantsWithStripeCustomer();
        var service = Service(tenants);

        var result = await service.HandleEventAsync(new StripeEvent
        {
            Type = "invoice.payment_failed",
            CustomerId = CustomerId
        });

        Assert.Equal(SubscriptionStatus.PastDue, result.NewStatus);
    }

    [Fact]
    public async Task HandleEventAsync_ReturnsTenantNotFound_ForUnknownCustomer()
    {
        var service = Service(new InMemoryTenantRepository());

        var result = await service.HandleEventAsync(new StripeEvent
        {
            Type = "checkout.session.completed",
            CustomerId = "cus_unknown"
        });

        Assert.Equal(BillingWebhookOutcome.TenantNotFound, result.Outcome);
    }

    [Fact]
    public async Task HandleEventAsync_IgnoresIrrelevantEvent()
    {
        var tenants = await TenantsWithStripeCustomer();
        var service = Service(tenants);

        var result = await service.HandleEventAsync(new StripeEvent
        {
            Type = "customer.created",
            CustomerId = CustomerId
        });

        Assert.Equal(BillingWebhookOutcome.Ignored, result.Outcome);
    }

    // ── Portail client ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePortalUrlAsync_ReturnsUrl_WhenCustomerLinked()
    {
        var tenants = await TenantsWithStripeCustomer();
        var service = Service(tenants);

        var url = await service.CreatePortalUrlAsync(TenantId, "https://app/return");

        Assert.NotNull(url);
        Assert.Contains(CustomerId, url);
    }

    [Fact]
    public async Task CreatePortalUrlAsync_ReturnsNull_WhenNoCustomer()
    {
        // Cabinet de démo sans StripeCustomerId (null par défaut).
        var service = Service(new InMemoryTenantRepository());

        var url = await service.CreatePortalUrlAsync(TenantId, "https://app/return");

        Assert.Null(url);
    }
}
