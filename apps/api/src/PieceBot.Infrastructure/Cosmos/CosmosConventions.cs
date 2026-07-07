using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;

namespace PieceBot.Infrastructure.Cosmos;

/// <summary>Constantes de nommage de la base et des conteneurs Cosmos (spec §4).</summary>
internal static class CosmosNames
{
    public const string Database = "piecebot";
    public const string Tenants = "tenants";
    public const string EndClients = "endClients";
    public const string Pieces = "pieces";

    /// <summary>Clé de partition commune (isolation multi-tenant, spec §3.2).</summary>
    public const string PartitionKeyPath = "/tenantId";
}

/// <summary>
/// Sérialiseur Cosmos basé sur System.Text.Json, aligné sur le contrat frontend :
/// propriétés camelCase, enums en snake_case (ex. <c>invoice_purchase</c>). Évite
/// la dépendance implicite à Newtonsoft du SDK et garantit un stockage cohérent
/// avec les réponses de l'API.
/// </summary>
internal sealed class SystemTextJsonCosmosSerializer : CosmosSerializer
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            return JsonSerializer.Deserialize<T>(stream, Options)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, input, Options);
        stream.Position = 0;
        return stream;
    }
}
