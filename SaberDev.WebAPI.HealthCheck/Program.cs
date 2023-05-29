using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SaberDev.WebAPI.HealthCheck.Db;
using SaberDev.WebAPI.HealthCheck.HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHealthChecks().AddTypeActivatedCheck<DatabaseHealthCheck>("database_health_check");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddTransient<IDbConnection>(opt =>
    {
        var configuration = opt.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Default");
        return new SqliteConnection(connectionString);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<AppDbContext>>();
    logger.LogError(ex, "An error occured during migration.");
}

app.Run();
