using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure;

/// <summary>
/// Données de démonstration (cabinet Mvogo + clients), le temps de brancher
/// Cosmos DB. Miroir du jeu de données du frontend.
/// </summary>
internal static class DemoData
{
    public const string TenantId = "tenant_mvogo";
    public const string CurrentMonth = "2026-06";

    public static IEnumerable<EndClient> EndClients() =>
    [
        new()
        {
            Id = "client_aissa",
            TenantId = TenantId,
            CompanyName = "SARL Boutique Aïssa",
            ContactName = "Aïssa Diallo",
            WhatsappNumber = "+237699123456",
            Email = "aissa@boutique.cm",
            Siret = "M032018123456",
            VatNumber = "P123456789",
            Tags = ["commerce", "actif"],
            ActiveMonth = CurrentMonth,
            CreatedAt = DateTimeOffset.Parse("2026-05-02T10:00:00Z"),
            LastPieceReceived = DateTimeOffset.Parse("2026-06-28T15:32:00Z")
        },
        new()
        {
            Id = "client_batipro",
            TenantId = TenantId,
            CompanyName = "BatiPro Douala",
            ContactName = "Serge Nkomo",
            WhatsappNumber = "+237677889900",
            Email = "serge@batipro.cm",
            Siret = "M052019998877",
            VatNumber = "P987654321",
            Tags = ["btp"],
            ActiveMonth = CurrentMonth,
            CreatedAt = DateTimeOffset.Parse("2026-04-18T10:00:00Z"),
            LastPieceReceived = DateTimeOffset.Parse("2026-06-27T09:14:00Z")
        },
        new()
        {
            Id = "client_pharma",
            TenantId = TenantId,
            CompanyName = "Pharmacie du Wouri",
            ContactName = "Dr. Mballa",
            WhatsappNumber = "+237698112233",
            Email = null,
            Siret = "M012017445566",
            VatNumber = null,
            Tags = ["sante"],
            ActiveMonth = CurrentMonth,
            CreatedAt = DateTimeOffset.Parse("2026-03-10T10:00:00Z"),
            LastPieceReceived = DateTimeOffset.Parse("2026-06-10T11:02:00Z")
        },
        new()
        {
            Id = "client_transport",
            TenantId = TenantId,
            CompanyName = "Transports Étoile",
            ContactName = "Paul Etoga",
            WhatsappNumber = "+237691445566",
            Email = "paul@etoile-transport.cm",
            Siret = "M072020334455",
            VatNumber = "P456789123",
            Tags = ["transport", "retard"],
            ActiveMonth = CurrentMonth,
            CreatedAt = DateTimeOffset.Parse("2026-05-20T10:00:00Z"),
            LastPieceReceived = null
        }
    ];

    private static string Blob(string id, string ext = "jpg") =>
        $"https://piecebot.blob.core.windows.net/pieces/{TenantId}/{CurrentMonth}/{id}.{ext}";

    public static IEnumerable<Piece> Pieces() =>
    [
        new()
        {
            Id = "piece_001",
            TenantId = TenantId,
            EndClientId = "client_aissa",
            Category = PieceCategory.InvoicePurchase,
            Status = PieceStatus.Extracted,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-28T15:32:00Z"),
            BlobUrl = Blob("piece_001"),
            MimeType = "image/jpeg",
            OriginalFileName = "IMG_20260628.jpg",
            ExtractedData = new ExtractedData
            {
                Supplier = "ENEO Cameroun",
                DocumentDate = new DateOnly(2026, 6, 15),
                DocumentNumber = "FAC-2026-08-12345",
                TotalAmountTtc = 78500,
                TotalAmountHt = 65798,
                TotalVat = 12702,
                Currency = "XAF"
            },
            Confidence = 0.92,
            OcrRawText = "ENEO CAMEROUN — Facture electricite ...",
            WhatsappMessageId = "wamid.aaa"
        },
        new()
        {
            Id = "piece_002",
            TenantId = TenantId,
            EndClientId = "client_aissa",
            Category = PieceCategory.Receipt,
            Status = PieceStatus.Extracted,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-26T08:10:00Z"),
            BlobUrl = Blob("piece_002"),
            MimeType = "image/jpeg",
            OriginalFileName = "ticket_marche.jpg",
            ExtractedData = new ExtractedData
            {
                Supplier = "Marché Mboppi",
                DocumentDate = new DateOnly(2026, 6, 25),
                TotalAmountTtc = 24000,
                Currency = "XAF"
            },
            Confidence = 0.61,
            OcrRawText = "TICKET ...",
            WhatsappMessageId = "wamid.bbb"
        },
        new()
        {
            Id = "piece_003",
            TenantId = TenantId,
            EndClientId = "client_aissa",
            Category = PieceCategory.InvoicePurchase,
            Status = PieceStatus.Validated,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-12T14:00:00Z"),
            BlobUrl = Blob("piece_003", "pdf"),
            MimeType = "application/pdf",
            OriginalFileName = "facture_camtel.pdf",
            ExtractedData = new ExtractedData
            {
                Supplier = "CAMTEL",
                DocumentDate = new DateOnly(2026, 6, 5),
                DocumentNumber = "CT-556677",
                TotalAmountTtc = 45000,
                TotalAmountHt = 37735,
                TotalVat = 7265,
                Currency = "XAF"
            },
            Confidence = 0.97,
            ValidatedBy = "clerk_user_id",
            ValidatedAt = DateTimeOffset.Parse("2026-06-13T09:20:00Z"),
            OcrRawText = "CAMTEL ...",
            WhatsappMessageId = "wamid.ccc"
        },
        new()
        {
            Id = "piece_004",
            TenantId = TenantId,
            EndClientId = "client_batipro",
            Category = PieceCategory.InvoicePurchase,
            Status = PieceStatus.Extracted,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-27T09:14:00Z"),
            BlobUrl = Blob("piece_004"),
            MimeType = "image/jpeg",
            OriginalFileName = "ciment.jpg",
            ExtractedData = new ExtractedData
            {
                Supplier = "Cimencam",
                DocumentDate = new DateOnly(2026, 6, 24),
                DocumentNumber = "CIM-2026-4521",
                TotalAmountTtc = 1250000,
                TotalAmountHt = 1048218,
                TotalVat = 201782,
                Currency = "XAF"
            },
            Confidence = 0.88,
            OcrRawText = "CIMENCAM ...",
            WhatsappMessageId = "wamid.ddd"
        },
        new()
        {
            Id = "piece_005",
            TenantId = TenantId,
            EndClientId = "client_batipro",
            Category = PieceCategory.BankStatement,
            Status = PieceStatus.Received,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-27T09:16:00Z"),
            BlobUrl = Blob("piece_005", "pdf"),
            MimeType = "application/pdf",
            OriginalFileName = "releve_afriland.pdf",
            ExtractedData = new ExtractedData(),
            Confidence = 0,
            WhatsappMessageId = "wamid.eee"
        },
        new()
        {
            Id = "piece_006",
            TenantId = TenantId,
            EndClientId = "client_pharma",
            Category = PieceCategory.InvoicePurchase,
            Status = PieceStatus.Validated,
            Month = CurrentMonth,
            ReceivedAt = DateTimeOffset.Parse("2026-06-10T11:02:00Z"),
            BlobUrl = Blob("piece_006"),
            MimeType = "image/jpeg",
            OriginalFileName = "fournisseur_pharma.jpg",
            ExtractedData = new ExtractedData
            {
                Supplier = "Laborex Cameroun",
                DocumentDate = new DateOnly(2026, 6, 8),
                DocumentNumber = "LBX-99812",
                TotalAmountTtc = 890000,
                TotalAmountHt = 746331,
                TotalVat = 143669,
                Currency = "XAF"
            },
            Confidence = 0.94,
            ValidatedBy = "clerk_user_id",
            ValidatedAt = DateTimeOffset.Parse("2026-06-11T08:00:00Z"),
            OcrRawText = "LABOREX ...",
            WhatsappMessageId = "wamid.fff"
        }
    ];

    public static Tenant Tenant() => new()
    {
        Id = TenantId,
        TenantId = TenantId,
        Name = "Cabinet Mvogo & Associés",
        Country = "CM",
        Currency = "XAF",
        VatRate = 19.25m,
        WhatsappPhoneNumberId = "1234567890",
        WhatsappBusinessAccountId = "9876543210",
        SubscriptionStatus = SubscriptionStatus.Trialing,
        SubscriptionPlan = SubscriptionPlan.Standard,
        StripeCustomerId = null,
        TrialEndsAt = DateTimeOffset.Parse("2026-07-13T00:00:00Z"),
        CreatedAt = DateTimeOffset.Parse("2026-06-29T10:00:00Z"),
        Owner = new TenantOwner
        {
            UserId = "clerk_user_id",
            Email = "jean@cabinet-mvogo.cm",
            FullName = "Jean Mvogo"
        },
        Settings = new TenantSettings
        {
            Language = "fr",
            Timezone = "Africa/Douala",
            MonthlyReminderDay = 5,
            MonthlyReminderHour = 9
        }
    };
}
