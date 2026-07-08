using ClosedXML.Excel;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PieceBot.Infrastructure.Exports;

/// <summary>
/// Rendu réel des dossiers mensuels : PDF via QuestPDF, Excel via ClosedXML
/// (spec §5.7). Aucune dépendance cloud — la génération est purement locale et
/// synchrone (CPU), ce qui tient largement le critère « export &lt; 30 s ».
/// </summary>
public sealed class PdfExcelExportRenderer : IExportRenderer
{
    private const string Green = "#0B8A44";
    private const string Ink = "#0C1117";
    private const string Muted = "#6B7A71";
    private const string Border = "#E4EBE2";
    private const string White = "#FFFFFF";

    static PdfExcelExportRenderer()
    {
        // Licence Community (gratuite en dessous d'1 M$ de CA) — requise par QuestPDF.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ExportFile Render(
        ExportJob job,
        IReadOnlyList<Piece> pieces,
        IReadOnlyDictionary<string, string> clientNames)
    {
        var scopeName = job.EndClientId is not null && clientNames.TryGetValue(job.EndClientId, out var n)
            ? n
            : "Tout le cabinet";

        return job.Format == ExportFormat.Pdf
            ? RenderPdf(job, pieces, clientNames, scopeName)
            : RenderExcel(job, pieces, clientNames, scopeName);
    }

    private static ExportFile RenderPdf(
        ExportJob job,
        IReadOnlyList<Piece> pieces,
        IReadOnlyDictionary<string, string> clientNames,
        string scopeName)
    {
        var currency = pieces.Select(p => p.ExtractedData.Currency).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c));

        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(t => t.FontSize(9).FontColor(Ink));

                page.Header().Column(col =>
                {
                    col.Item().Text("PieceBot").FontSize(18).Bold().FontColor(Green);
                    col.Item().Text($"Dossier mensuel · {job.Month}").FontSize(12).SemiBold();
                    col.Item().Text(scopeName).FontColor(Muted);
                });

                page.Content().PaddingVertical(12).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);    // Client
                        c.RelativeColumn(2);    // Fournisseur
                        c.ConstantColumn(52);   // Date
                        c.RelativeColumn(1.4f);  // Catégorie
                        c.ConstantColumn(66);   // HT
                        c.ConstantColumn(56);   // TVA
                        c.ConstantColumn(74);   // TTC
                    });

                    table.Header(h =>
                    {
                        void HeadL(string t) => h.Cell().Element(HeadCell).Text(t).SemiBold().FontColor(White);
                        void HeadR(string t) => h.Cell().Element(HeadCell).AlignRight().Text(t).SemiBold().FontColor(White);
                        HeadL("Client"); HeadL("Fournisseur"); HeadL("Date"); HeadL("Catégorie");
                        HeadR("HT"); HeadR("TVA"); HeadR("TTC");
                    });

                    foreach (var p in pieces)
                    {
                        var ex = p.ExtractedData;
                        var client = clientNames.TryGetValue(p.EndClientId, out var cn) ? cn : p.EndClientId;
                        table.Cell().Element(BodyCell).Text(client);
                        table.Cell().Element(BodyCell).Text(ex.Supplier ?? "—");
                        table.Cell().Element(BodyCell).Text(ex.DocumentDate?.ToString("dd/MM/yy") ?? "—");
                        table.Cell().Element(BodyCell).Text(CategoryLabel(p.Category));
                        table.Cell().Element(BodyCell).AlignRight().Text(Money(ex.TotalAmountHt, ex.Currency));
                        table.Cell().Element(BodyCell).AlignRight().Text(Money(ex.TotalVat, ex.Currency));
                        table.Cell().Element(BodyCell).AlignRight().Text(Money(ex.TotalAmountTtc, ex.Currency));
                    }
                });

                page.Footer().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Text($"{pieces.Count} pièce(s) · Total TTC {Money(job.TotalAmountTtc, currency)}")
                        .SemiBold();
                    row.RelativeItem().AlignRight().Text($"Généré le {DateTimeOffset.Now:dd/MM/yyyy HH:mm}")
                        .FontColor(Muted);
                });
            });
        }).GeneratePdf();

        return new ExportFile(bytes, FileName(job, scopeName, "pdf"), "application/pdf");
    }

    private static ExportFile RenderExcel(
        ExportJob job,
        IReadOnlyList<Piece> pieces,
        IReadOnlyDictionary<string, string> clientNames,
        string scopeName)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add($"Dossier {job.Month}");

        ws.Cell(1, 1).Value = "PieceBot — Dossier mensuel";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(2, 1).Value = $"{scopeName} · {job.Month}";
        ws.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml(Muted);

        const int headerRow = 4;
        string[] headers = { "Client", "Fournisseur", "Date", "N° pièce", "Catégorie", "HT", "TVA", "TTC", "Devise", "Statut" };
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(headerRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(Green);
        }

        var r = headerRow + 1;
        foreach (var p in pieces)
        {
            var ex = p.ExtractedData;
            ws.Cell(r, 1).Value = clientNames.TryGetValue(p.EndClientId, out var cn) ? cn : p.EndClientId;
            ws.Cell(r, 2).Value = ex.Supplier ?? "";
            if (ex.DocumentDate is { } d) ws.Cell(r, 3).Value = d.ToDateTime(TimeOnly.MinValue);
            ws.Cell(r, 3).Style.DateFormat.Format = "dd/mm/yyyy";
            ws.Cell(r, 4).Value = ex.DocumentNumber ?? "";
            ws.Cell(r, 5).Value = CategoryLabel(p.Category);
            ws.Cell(r, 6).Value = MoneyValue(ex.TotalAmountHt, ex.Currency);
            ws.Cell(r, 7).Value = MoneyValue(ex.TotalVat, ex.Currency);
            ws.Cell(r, 8).Value = MoneyValue(ex.TotalAmountTtc, ex.Currency);
            ws.Cell(r, 9).Value = string.IsNullOrWhiteSpace(ex.Currency) ? "XAF" : ex.Currency;
            ws.Cell(r, 10).Value = p.Status.ToString();
            r++;
        }

        // Ligne de total.
        ws.Cell(r, 5).Value = "Total TTC";
        ws.Cell(r, 5).Style.Font.Bold = true;
        ws.Cell(r, 8).Value = MoneyValue(job.TotalAmountTtc, pieces.Select(p => p.ExtractedData.Currency).FirstOrDefault());
        ws.Cell(r, 8).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return new ExportFile(
            ms.ToArray(),
            FileName(job, scopeName, "xlsx"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static IContainer HeadCell(IContainer c) =>
        c.Background(Green).PaddingVertical(4).PaddingHorizontal(5);

    private static IContainer BodyCell(IContainer c) =>
        c.BorderBottom(0.5f).BorderColor(Border).PaddingVertical(3).PaddingHorizontal(5);

    private static string FileName(ExportJob job, string scopeName, string ext)
    {
        var slug = Slug(job.EndClientId is null ? "cabinet" : scopeName);
        return $"piecebot_{job.Month}_{slug}.{ext}";
    }

    private static string Slug(string value)
    {
        var chars = value.ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray();
        return new string(chars).Trim('-');
    }

    /// <summary>Devises « zéro décimale » d'Afrique francophone : le montant stocké est déjà l'unité entière.</summary>
    private static bool IsZeroDecimal(string code) =>
        code is "XAF" or "XOF" or "KMF" or "GNF" or "JPY";

    private static decimal MoneyValue(long? minor, string? currency)
    {
        var code = string.IsNullOrWhiteSpace(currency) ? "XAF" : currency.Trim().ToUpperInvariant();
        return (minor ?? 0) / (IsZeroDecimal(code) ? 1m : 100m);
    }

    private static string Money(long? minor, string? currency)
    {
        var code = string.IsNullOrWhiteSpace(currency) ? "XAF" : currency.Trim().ToUpperInvariant();
        return $"{MoneyValue(minor, currency):#,##0.##} {code}";
    }

    private static string CategoryLabel(PieceCategory category) => category switch
    {
        PieceCategory.InvoicePurchase => "Facture d'achat",
        PieceCategory.InvoiceSale => "Facture de vente",
        PieceCategory.Receipt => "Reçu",
        PieceCategory.BankStatement => "Relevé bancaire",
        PieceCategory.PaymentProof => "Preuve de paiement",
        PieceCategory.Other => "Autre",
        _ => category.ToString()
    };
}
