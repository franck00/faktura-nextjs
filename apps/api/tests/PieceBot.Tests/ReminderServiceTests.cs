using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class ReminderServiceTests
{
    private const string TenantId = "tenant_mvogo";
    private const string Month = "2026-06";

    private static (ReminderService service, InMemoryEndClientRepository clients) NewService()
    {
        var clients = new InMemoryEndClientRepository();
        var service = new ReminderService(
            new InMemoryTenantRepository(),
            clients,
            new InMemoryPieceRepository(),
            new InMemoryMonthlyReminderRepository());
        return (service, clients);
    }

    [Fact]
    public async Task GetConfigAsync_ReturnsTenantSettings()
    {
        var (service, _) = NewService();

        var config = await service.GetConfigAsync(TenantId);

        Assert.NotNull(config);
        Assert.Equal(5, config!.MonthlyReminderDay);
        Assert.Equal(9, config.MonthlyReminderHour);
        Assert.True(config.Enabled);
        Assert.False(string.IsNullOrWhiteSpace(config.MessageTemplate));
    }

    [Fact]
    public async Task GetConfigAsync_ReturnsNull_ForUnknownTenant()
    {
        var (service, _) = NewService();

        var config = await service.GetConfigAsync("tenant_unknown");

        Assert.Null(config);
    }

    [Fact]
    public async Task UpdateConfigAsync_PersistsValues()
    {
        var (service, _) = NewService();

        var updated = await service.UpdateConfigAsync(TenantId, new ReminderConfig
        {
            MonthlyReminderDay = 10,
            MonthlyReminderHour = 7,
            Enabled = false,
            MessageTemplate = "Rappel personnalisé {client}"
        });

        Assert.NotNull(updated);
        Assert.Equal(10, updated!.MonthlyReminderDay);
        Assert.Equal(7, updated.MonthlyReminderHour);
        Assert.False(updated.Enabled);
        Assert.Equal("Rappel personnalisé {client}", updated.MessageTemplate);
    }

    [Theory]
    [InlineData(0, 9)]
    [InlineData(31, 9)]
    [InlineData(5, -1)]
    [InlineData(5, 24)]
    public async Task UpdateConfigAsync_Throws_ForInvalidRange(int day, int hour)
    {
        var (service, _) = NewService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateConfigAsync(TenantId, new ReminderConfig
            {
                MonthlyReminderDay = day,
                MonthlyReminderHour = hour,
                Enabled = true,
                MessageTemplate = "x"
            }));
    }

    [Fact]
    public async Task SendNowAsync_CreatesReminders_ForAllActiveClients()
    {
        var (service, _) = NewService();

        var result = await service.SendNowAsync(TenantId, Month);

        Assert.NotNull(result);
        Assert.False(result!.Disabled);
        Assert.Equal(4, result.Sent);
        Assert.Equal(0, result.Skipped);
        Assert.All(result.Reminders, r => Assert.Equal(1, r.RemindersSent));
    }

    [Fact]
    public async Task SendNowAsync_IncrementsRemindersSent_OnSecondRun()
    {
        var (service, _) = NewService();

        await service.SendNowAsync(TenantId, Month);
        var second = await service.SendNowAsync(TenantId, Month);

        Assert.NotNull(second);
        Assert.All(second!.Reminders, r => Assert.Equal(2, r.RemindersSent));
    }

    [Fact]
    public async Task SendNowAsync_SkipsDisabledClients()
    {
        var (service, clients) = NewService();
        await clients.CreateAsync(new EndClient
        {
            Id = "client_disabled",
            TenantId = TenantId,
            CompanyName = "Client Sans Rappel",
            ContactName = "Test",
            WhatsappNumber = "+237600000099",
            ActiveMonth = Month,
            CreatedAt = DateTimeOffset.UtcNow,
            RemindersDisabled = true
        });

        var result = await service.SendNowAsync(TenantId, Month);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Skipped);
        Assert.DoesNotContain(result.Reminders, r => r.EndClientId == "client_disabled");
    }

    [Fact]
    public async Task SendNowAsync_ComputesCompletionRate()
    {
        var (service, _) = NewService();

        var result = await service.SendNowAsync(TenantId, Month);

        // Pharmacie du Wouri : 1 pièce reçue et validée → 1.0.
        var pharma = result!.Reminders.Single(r => r.EndClientId == "client_pharma");
        Assert.Equal(1d, pharma.CompletionRate);

        // Transports Étoile : aucune pièce → 0.
        var transport = result.Reminders.Single(r => r.EndClientId == "client_transport");
        Assert.Equal(0d, transport.CompletionRate);
    }

    [Fact]
    public async Task SendNowAsync_ReturnsDisabled_WhenRemindersOff()
    {
        var (service, _) = NewService();
        await service.UpdateConfigAsync(TenantId, new ReminderConfig
        {
            MonthlyReminderDay = 5,
            MonthlyReminderHour = 9,
            Enabled = false,
            MessageTemplate = "x"
        });

        var result = await service.SendNowAsync(TenantId, Month);

        Assert.NotNull(result);
        Assert.True(result!.Disabled);
        Assert.Equal(0, result.Sent);
    }

    [Fact]
    public async Task SendNowAsync_ReturnsNull_ForUnknownTenant()
    {
        var (service, _) = NewService();

        var result = await service.SendNowAsync("tenant_unknown", Month);

        Assert.Null(result);
    }
}
