namespace PieceBot.Api.Contracts;

/// <summary>Payload de création d'un client du cabinet (POST /api/endclients).</summary>
public sealed record CreateEndClientRequest(
    string CompanyName,
    string ContactName,
    string WhatsappNumber,
    string? Email,
    string? Siret,
    string? VatNumber,
    List<string>? Tags,
    string? ActiveMonth);

/// <summary>Payload de mise à jour d'un client (PUT /api/endclients/{id}).</summary>
public sealed record UpdateEndClientRequest(
    string CompanyName,
    string ContactName,
    string WhatsappNumber,
    string? Email,
    string? Siret,
    string? VatNumber,
    List<string>? Tags,
    string? ActiveMonth);
