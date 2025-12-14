using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Repositories;
using prisonbreak.Server.Services;
using prisonbreak.Server.Models;
using prisonbreak.Server.Hubs;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Configuration du port pour Render (obligatoire)
// Render injecte la variable PORT, on doit l'utiliser
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    // Sur Render, écouter sur 0.0.0.0 (toutes les interfaces) et le port fourni
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

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
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();

// Configuration SignalR pour le chat en temps réel
builder.Services.AddSignalR();

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
// En développement, on autorise les ports de Vite (5173 et 5174 pour le multijoueur), le proxy SPA, et ngrok
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:5173",
            "https://localhost:5173",
            "http://localhost:5174",
            "https://localhost:5174",
            "http://localhost:5000",
            "https://localhost:5001"
        };

        // Ajouter les URLs ngrok si configurées via variable d'environnement
        var ngrokUrls = Environment.GetEnvironmentVariable("NGROK_URLS");
        if (!string.IsNullOrEmpty(ngrokUrls))
        {
            var urls = ngrokUrls.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var url in urls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    allowedOrigins.Add(url.Trim());
                    // Ajouter aussi la version HTTPS si c'est HTTP
                    if (url.Trim().StartsWith("http://"))
                    {
                        allowedOrigins.Add(url.Trim().Replace("http://", "https://"));
                    }
                }
            }
        }

        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        
        // En développement, autoriser aussi toutes les URLs ngrok (format *.ngrok.io, *.ngrok-free.app, etc.)
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;
                
                // Autoriser localhost
                if (origin.Contains("localhost") || origin.Contains("127.0.0.1"))
                    return true;
                
                // Autoriser les domaines ngrok
                if (origin.Contains(".ngrok.io") || 
                    origin.Contains(".ngrok-free.app") ||
                    origin.Contains(".ngrok.app") ||
                    origin.Contains("ngrok.io") ||
                    origin.Contains("ngrok-free.app") ||
                    origin.Contains("ngrok.app"))
                    return true;
                
                return false;
            });
        }
    });
});

// Note: Le SPA Proxy est configuré automatiquement via le .csproj
// Les propriétés SpaRoot, SpaProxyServerUrl et SpaProxyLaunchCommand
// permettent à Visual Studio de lancer automatiquement le client Vue.js

// Configuration pour servir les fichiers statiques (images uploadées)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

var app = builder.Build();

// ====================================================
// NOTE: Le frontend est lancé automatiquement par le SPA Proxy de Visual Studio
// ====================================================
// Le SPA Proxy (Microsoft.AspNetCore.SpaProxy) lance automatiquement
// le frontend via la configuration dans .csproj et launchSettings.json
// Pas besoin de script PowerShell supplémentaire

// ====================================================
// LANCEMENT AUTOMATIQUE DE LA 2ÈME INSTANCE DU FRONTEND (MULTIJOUEUR)
// ====================================================
// En mode développement, lancer automatiquement une 2ème instance sur le port 5174
if (app.Environment.IsDevelopment())
{
    try
    {
        var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "frontend");
        frontendPath = Path.GetFullPath(frontendPath);
        
        if (Directory.Exists(frontendPath))
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("🚀 Lancement de l'instance 2 du frontend sur le port 5174...");
            
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoExit -Command \"cd '{frontendPath}'; Write-Host '=== INSTANCE 2 - PORT 5174 ===' -ForegroundColor Blue; npm run dev:port2\"",
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = frontendPath
            };
            
            System.Diagnostics.Process.Start(processInfo);
            logger.LogInformation("✅ Instance 2 du frontend lancée sur http://localhost:5174");
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "⚠️ Impossible de lancer l'instance 2 du frontend. Lancez-la manuellement : cd frontend && npm run dev:port2");
    }
}

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

// IMPORTANT: UseRouting() doit être appelé avant MapControllers() et UseStaticFiles()
// pour que le routing fonctionne correctement
app.UseRouting();

// Activer CORS pour le frontend (après UseRouting, avant les endpoints)
app.UseCors("AllowVueFrontend");

// Configuration des endpoints (après UseRouting et UseCors)
// IMPORTANT: Tous les Map* doivent être appelés après UseRouting()

// Configuration SignalR
app.MapHub<prisonbreak.Server.Hubs.ChatHub>("/hubs/chat");

// Utiliser les contrôleurs API
app.MapControllers();

// Servir les fichiers statiques (nécessaire pour les uploads d'images)
// IMPORTANT: Utiliser MapWhen pour exclure les routes /api/* et /hubs/*
// Sur Render, le frontend est un service séparé, donc on ne sert que les fichiers statiques
app.MapWhen(context => !(context.Request.Path.Value ?? "").StartsWith("/api/", StringComparison.OrdinalIgnoreCase) &&
                       !(context.Request.Path.Value ?? "").StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase),
    appBuilder => appBuilder.UseStaticFiles());

// Log pour indiquer que le serveur est prêt
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
var portEnv = Environment.GetEnvironmentVariable("PORT");
startupLogger.LogInformation("========================================");
startupLogger.LogInformation("Serveur backend demarre sur:");
if (!string.IsNullOrEmpty(portEnv))
{
    startupLogger.LogInformation($"  HTTP:  http://0.0.0.0:{portEnv} (Render)");
}
else
{
    startupLogger.LogInformation("  HTTP:  http://localhost:5000");
    startupLogger.LogInformation("  HTTPS: https://localhost:5001");
    startupLogger.LogInformation("  Swagger: https://localhost:5001/swagger");
}
startupLogger.LogInformation("========================================");

app.Run();
