namespace PieceBot.Core.Domain;

/// <summary>
/// Entreprise cliente du cabinet comptable (collection Cosmos <c>endClients</c>,
/// spec §4.2). Partitionnée par <see cref="TenantId"/>.
/// </summary>
public sealed class EndClient
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public string Type { get; init; } = "endclient";

    public required string CompanyName { get; set; }
    public required string ContactName { get; set; }
    public required string WhatsappNumber { get; set; }
    public string? Email { get; set; }
    public string? Siret { get; set; }
    public string? VatNumber { get; set; }
    public List<string> Tags { get; set; } = [];

    /// <summary>Mois comptable de rattachement, format <c>YYYY-MM</c>.</summary>
    public required string ActiveMonth { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? LastPieceReceived { get; set; }
}
