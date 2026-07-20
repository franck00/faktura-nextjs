using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Ocr;

/// <summary>
/// Extraction OCR réelle via Azure Document Intelligence (modèle prébâti
/// <c>prebuilt-receipt</c>, spec §3.2). Activé quand <c>DocumentIntelligence:Endpoint</c>
/// + <c>Key</c> sont configurés (sinon <see cref="NoOpReceiptExtractor"/>).
/// </summary>
public sealed class AzureDocumentIntelligenceExtractor : IReceiptExtractor
{
    private readonly DocumentIntelligenceClient _client;

    public AzureDocumentIntelligenceExtractor(IConfiguration configuration)
    {
        var endpoint = configuration["DocumentIntelligence:Endpoint"]
            ?? throw new InvalidOperationException("DocumentIntelligence:Endpoint manquant.");
        var key = configuration["DocumentIntelligence:Key"]
            ?? throw new InvalidOperationException("DocumentIntelligence:Key manquant.");
        _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
    }

    public async Task<ReceiptExtraction> ExtractAsync(
        byte[] content,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        var options = new AnalyzeDocumentOptions("prebuilt-receipt", BinaryData.FromBytes(content));
        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, options, cancellationToken);
        var document = operation.Value.Documents.FirstOrDefault();
        if (document is null)
        {
            return ReceiptExtraction.Empty;
        }

        var data = new ExtractedData();
        string? currency = null;

        if (document.Fields.TryGetValue("MerchantName", out var merchant))
        {
            data.Supplier = merchant.ValueString;
        }

        if (document.Fields.TryGetValue("TransactionDate", out var date)
            && date.FieldType == DocumentFieldType.Date
            && date.ValueDate is { } d)
        {
            data.DocumentDate = DateOnly.FromDateTime(d.DateTime);
        }

        if (document.Fields.TryGetValue("Total", out var total))
        {
            (data.TotalAmountTtc, currency) = ReadMoney(total);
        }

        if (document.Fields.TryGetValue("TotalTax", out var tax))
        {
            (data.TotalVat, _) = ReadMoney(tax);
        }

        if (document.Fields.TryGetValue("Subtotal", out var subtotal))
        {
            (data.TotalAmountHt, _) = ReadMoney(subtotal);
        }

        data.Currency = currency;

        return new ReceiptExtraction(data, document.Confidence, PieceCategory.InvoicePurchase);
    }

    /// <summary>Convertit un champ montant en unités mineures (FCFA = pas de décimale).</summary>
    private static (long?, string?) ReadMoney(DocumentField field)
    {
        if (field.FieldType == DocumentFieldType.Currency && field.ValueCurrency is { } money)
        {
            var code = string.IsNullOrWhiteSpace(money.CurrencyCode) ? "XAF" : money.CurrencyCode;
            var minor = IsZeroDecimal(code)
                ? (long)Math.Round(money.Amount)
                : (long)Math.Round(money.Amount * 100);
            return (minor, code);
        }

        if (field.FieldType == DocumentFieldType.Double && field.ValueDouble is { } value)
        {
            return ((long)Math.Round(value), null);
        }

        return (null, null);
    }

    private static bool IsZeroDecimal(string code) =>
        code is "XAF" or "XOF" or "KMF" or "GNF" or "JPY";
}
