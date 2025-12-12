using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Repositories;
using prisonbreak.Server.Services;
using prisonbreak.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// ====================================================
// CONFIGURATION DES SERVICES
// ====================================================

// Configuration de la base de données SQLite
// SQLite est utilisé pour sa simplicité et portabilité
// Pour la production, envisagez PostgreSQL ou SQL Server
builder.Services.AddDbContext<HashiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=hashi.db"));

// ====================================================
// ENREGISTREMENT DES REPOSITORIES
// Pattern Repository : sépare l'accès aux données de la logique métier
// ====================================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// ====================================================
// ENREGISTREMENT DES SERVICES MÉTIER
// Ces services implémentent la logique du jeu Hashi
// Utilisent les repositories pour l'accès aux données
// ====================================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IPuzzleService, PuzzleService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ITicTacToeService, TicTacToeService>();
builder.Services.AddScoped<IConnectFourService, ConnectFourService>();
builder.Services.AddScoped<IRockPaperScissorsService, RockPaperScissorsService>();
builder.Services.AddScoped<IAdventureService, AdventureService>();
builder.Services.AddScoped<IItemService, ItemService>();

// Configuration des contrôleurs API
builder.Services.AddControllers();

// Configuration de Swagger pour la documentation API
// Accessible via /swagger quand l'application tourne
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Hashi API",
        Version = "v1",
        Description = "API pour le jeu de puzzle Hashi (Hashiwokakero)"
    });
});

// Configuration CORS pour permettre au frontend Vue.js de communiquer avec l'API
// En développement, on autorise les ports de Vite (5173 et 5174 pour le multijoueur) et le proxy SPA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5174",
                "http://localhost:5000",
                "https://localhost:5001"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Note: Le SPA Proxy est configuré automatiquement via le .csproj
// Les propriétés SpaRoot, SpaProxyServerUrl et SpaProxyLaunchCommand
// permettent à Visual Studio de lancer automatiquement le client Vue.js

var app = builder.Build();

// ====================================================
// NOTE: Le frontend est lancé automatiquement par le SPA Proxy de Visual Studio
// ====================================================
// Le SPA Proxy (Microsoft.AspNetCore.SpaProxy) lance automatiquement
// le frontend via la configuration dans .csproj et launchSettings.json
// Pas besoin de script PowerShell supplémentaire

// ====================================================
// INITIALISATION DE LA BASE DE DONNÉES
// ====================================================

// Appliquer les migrations automatiquement au démarrage
// En développement : applique les migrations automatiquement
// En production : utilisez 'dotnet ef database update' manuellement ou via un script de déploiement
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HashiDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Appliquer les migrations en attente
        logger.LogInformation("Application des migrations de base de données...");
        context.Database.Migrate();
        logger.LogInformation("Migrations appliquées avec succès.");

        // Initialiser les items de la boutique
        logger.LogInformation("Initialisation des items de la boutique...");
        await SeedItems.SeedAsync(context);
        logger.LogInformation("Items de la boutique initialisés.");

        // TODO: Ajouter des données de test ici si nécessaire
        // Par exemple : créer quelques puzzles de base
    }
    catch (Exception ex)
    {
        var errorLogger = services.GetRequiredService<ILogger<Program>>();
        errorLogger.LogError(ex, "Erreur lors de l'application des migrations de base de données");
    }
}

// ====================================================
// CONFIGURATION DU PIPELINE HTTP
// ====================================================

// En développement, activer Swagger pour tester l'API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hashi API v1");
        c.RoutePrefix = "swagger"; // Accessible via /swagger
    });
}

// Redirection HTTPS pour la sécurité (avant CORS)
app.UseHttpsRedirection();

// Activer CORS pour le frontend (après UseRouting si utilisé, avant UseEndpoints)
app.UseCors("AllowVueFrontend");

// En développement avec SPA Proxy, ne pas servir les fichiers statiques
// Le proxy redirige vers Vite qui sert les fichiers
// En production, servir les fichiers statiques compilés
if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

// Utiliser les contrôleurs API
app.MapControllers();

// En développement, le SPA Proxy (configuré dans .csproj) redirige automatiquement
// les requêtes non-API vers le serveur Vite qui tourne sur http://localhost:5173
// Visual Studio lance automatiquement "npm run dev" au démarrage

// Fallback vers index.html pour le routing côté client (SPA)
// IMPORTANT: Ne s'applique qu'en production ou si le SPA Proxy n'est pas actif
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("/index.html");
}

// Log pour indiquer que le serveur est prêt
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("========================================");
startupLogger.LogInformation("Serveur backend demarre sur:");
startupLogger.LogInformation("  HTTP:  http://localhost:5000");
startupLogger.LogInformation("  HTTPS: https://localhost:5001");
startupLogger.LogInformation("  Swagger: https://localhost:5001/swagger");
startupLogger.LogInformation("========================================");

app.Run();
