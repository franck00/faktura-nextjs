using System.Text.Json;
using System.Text.Json.Serialization;
using PieceBot.Core;
using PieceBot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Sérialiser les enums en chaînes (ex. "invoice_purchase") plutôt qu'en entiers.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)));

builder.Services.AddOpenApi();

// Couche Core / BLL (règles métier).
builder.Services.AddCoreServices();

// Couche Infrastructure (repositories, accès données : Cosmos si configuré, sinon In-Memory).
builder.Services.AddInfrastructure(builder.Configuration);

// CORS pour le frontend Next.js (apps/web).
const string WebCorsPolicy = "web";
builder.Services.AddCors(options =>
    options.AddPolicy(WebCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

// Provisionne la base + conteneurs Cosmos au démarrage (uniquement si Cosmos est configuré).
using (var scope = app.Services.CreateScope())
{
    var bootstrapper = scope.ServiceProvider.GetService<PieceBot.Infrastructure.Cosmos.CosmosBootstrapper>();
    if (bootstrapper is not null)
    {
        await bootstrapper.EnsureProvisionedAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(WebCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
