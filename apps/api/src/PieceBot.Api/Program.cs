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

// Couche Infrastructure (repositories, accès données).
builder.Services.AddInfrastructure();

// CORS pour le frontend Next.js (apps/web).
const string WebCorsPolicy = "web";
builder.Services.AddCors(options =>
    options.AddPolicy(WebCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(WebCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
