using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TodoApi.Data;
using Xunit;

namespace TodoApi.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private string? _connectionString;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();

        // Création de la base de données
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Démarre le container de manière synchrone si pas encore démarré
        if (_postgresContainer.State != TestcontainersStates.Running)
        {
            _postgresContainer.StartAsync().GetAwaiter().GetResult();
            _connectionString = _postgresContainer.GetConnectionString();
        }

        builder.ConfigureServices(services =>
        {
            // Supprime le DbContext existant
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Ajoute le DbContext avec la connection string du container
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString ?? _postgresContainer.GetConnectionString()));
        });
    }
}

