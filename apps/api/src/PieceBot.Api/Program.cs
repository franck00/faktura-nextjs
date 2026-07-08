using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

// Auth Clerk (JWT bearer) — activée uniquement si Clerk:Authority est configuré.
// Le tenant est résolu depuis les claims (voir TenantResolution) ; sans config,
// l'API reste ouverte avec le tenant de démo.
var clerkAuthority = builder.Configuration["Clerk:Authority"];
if (!string.IsNullOrWhiteSpace(clerkAuthority))
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = clerkAuthority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                NameClaimType = "sub"
            };
        });
}

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
if (!string.IsNullOrWhiteSpace(clerkAuthority))
{
    app.UseAuthentication();
}
app.UseAuthorization();
app.MapControllers();

app.Run();
