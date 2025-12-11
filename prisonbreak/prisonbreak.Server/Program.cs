using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Repositories;
using prisonbreak.Server.Services;

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
// LANCEMENT DES INSTANCES FRONTEND (Mode Debug uniquement)
// ====================================================
if (app.Environment.IsDevelopment())
{
    // Lancer les instances frontend en arrière-plan après un court délai
    // Utiliser le répertoire du projet source (où se trouve le .csproj)
    var projectDir = Directory.GetCurrentDirectory();
    var scriptPath = Path.Combine(projectDir, "launch-frontend-after-backend.ps1");
    
    if (File.Exists(scriptPath))
    {
        // Lancer le script en arrière-plan sans bloquer
        Task.Run(async () =>
        {
            // Attendre que le serveur soit complètement démarré
            await Task.Delay(3000);
            
            try
            {
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{scriptPath}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    WorkingDirectory = projectDir
                };
                System.Diagnostics.Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Impossible de lancer les instances frontend automatiquement");
            }
        });
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

        // TODO: Ajouter des données de test ici si nécessaire
        // Par exemple : créer quelques puzzles de base
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erreur lors de l'application des migrations de base de données");
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

app.Run();
