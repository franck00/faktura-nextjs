using PieceBot.Core.Domain;
using PieceBot.Core.Services;
using PieceBot.Infrastructure.Repositories;

namespace PieceBot.Tests;

public sealed class EndClientServiceTests
{
    private const string TenantId = "tenant_mvogo";

    [Fact]
    public async Task ListAsync_TrimsSearchTerm()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());

        var clients = await service.ListAsync(TenantId, "  batipro ");

        Assert.Single(clients);
        Assert.Contains("BatiPro", clients[0].CompanyName);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCompanyNameMissing()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());

        var invalid = new EndClient
        {
            Id = "client_new",
            TenantId = TenantId,
            CompanyName = "",
            ContactName = "Test Contact",
            WhatsappNumber = "+237600000000",
            ActiveMonth = "2026-06",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(invalid));
    }

    [Fact]
    public async Task ImportCsvAsync_CreatesClients_FromValidCsv()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());
        var csv = string.Join("\n",
            "companyName,contactName,whatsappNumber,email,tags,activeMonth",
            "Alpha SARL,Jean Alpha,+237600000001,jean@alpha.cm,commerce;actif,2026-06",
            "Beta SA,Awa Beta,+237600000002,,btp,2026-06");

        var result = await service.ImportCsvAsync(TenantId, csv);

        Assert.Equal(2, result.Created);
        Assert.Empty(result.Errors);
        Assert.Contains(result.Clients, c => c.CompanyName == "Alpha SARL" && c.Tags.Contains("actif"));
    }

    [Fact]
    public async Task ImportCsvAsync_ReportsError_ForRowMissingRequiredField()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());
        var csv = string.Join("\n",
            "companyName,contactName,whatsappNumber",
            ",Sans Nom,+237600000003"); // companyName manquant

        var result = await service.ImportCsvAsync(TenantId, csv);

        Assert.Equal(0, result.Created);
        Assert.Single(result.Errors);
        Assert.Equal(2, result.Errors[0].Line);
    }

    [Fact]
    public async Task ImportCsvAsync_InvalidHeader_ReturnsError()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());
        var csv = "nom,contact,tel\nAlpha,Jean,123";

        var result = await service.ImportCsvAsync(TenantId, csv);

        Assert.Equal(0, result.Created);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task ImportCsvAsync_HandlesQuotedFieldWithComma()
    {
        var service = new EndClientService(new InMemoryEndClientRepository());
        var csv = string.Join("\n",
            "companyName,contactName,whatsappNumber",
            "\"Alpha, Beta & Co\",Jean,+237600000004");

        var result = await service.ImportCsvAsync(TenantId, csv);

        Assert.Equal(1, result.Created);
        Assert.Equal("Alpha, Beta & Co", result.Clients[0].CompanyName);
    }
}
