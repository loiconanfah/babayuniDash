using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
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

// Enregistrement des services métier
// Ces services implémentent la logique du jeu Hashi
builder.Services.AddScoped<IPuzzleService, PuzzleService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

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
// En développement, on autorise le port par défaut de Vite (5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://localhost:5001")
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
// INITIALISATION DE LA BASE DE DONNÉES
// ====================================================

// Créer automatiquement la base de données si elle n'existe pas
// En développement seulement - en production, utilisez les migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HashiDbContext>();
        context.Database.EnsureCreated();
        
        // TODO: Ajouter des données de test ici si nécessaire
        // Par exemple : créer quelques puzzles de base
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erreur lors de la création de la base de données");
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

// Activer CORS pour le frontend
app.UseCors("AllowVueFrontend");

// Redirection HTTPS pour la sécurité
app.UseHttpsRedirection();

// Servir les fichiers statiques du frontend (après build)
app.UseDefaultFiles();
app.UseStaticFiles();

// Utiliser les contrôleurs API
app.MapControllers();

// En développement, le SPA Proxy (configuré dans .csproj) redirige automatiquement
// les requêtes non-API vers le serveur Vite qui tourne sur http://localhost:5173
// Visual Studio lance automatiquement "npm run dev" au démarrage

// Fallback vers index.html pour le routing côté client (SPA)
// Ceci permet à Vue Router de gérer les routes
app.MapFallbackToFile("/index.html");

app.Run();
