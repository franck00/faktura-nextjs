namespace PieceBot.Core.Domain;

/// <summary>Catégorie de pièce justificative reçue (cf. spec §4.3).</summary>
public enum PieceCategory
{
    InvoicePurchase,
    InvoiceSale,
    Receipt,
    BankStatement,
    PaymentProof,
    Other
}

/// <summary>Cycle de vie d'une pièce, de la réception WhatsApp à la validation.</summary>
public enum PieceStatus
{
    Received,
    Processing,
    Extracted,
    Validated,
    Rejected,
    Error
}
