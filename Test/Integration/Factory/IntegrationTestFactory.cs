using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation;
using Test.Integration.Authorization;
using Testcontainers.PostgreSql;

namespace Test.Integration.Factory;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string DatabaseName = "IntegrationTestDatabase";
    
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase(DatabaseName)
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly JwtFactory jwtFactory = new();
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        // Configure the host BEFORE it's built - setup initial config
        builder.ConfigureHostConfiguration(SetupTestConfiguration);

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment here
        builder.UseEnvironment("Testing");
        
        // setup config again because app config builder takes appsettings.Development.json and overrides existing config.
        builder.ConfigureAppConfiguration((_, config) => SetupTestConfiguration(config));

        base.ConfigureWebHost(builder);
    }

    private void SetupTestConfiguration(IConfigurationBuilder config)
    {
        // Clear existing sources that WebHost added
        config.Sources.Clear();

        // Add only test config
        config
            .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables();
        
        // Set environment variables (they override everything)
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__Secrets__0", jwtFactory.JwtSecret);
        Environment.SetEnvironmentVariable("Jwt__Issuer", TestUser.Issuer);
        Environment.SetEnvironmentVariable("Jwt__Audience", TestUser.Audience);
    }

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();
        
        // Optional: Run migrations
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await dbContainer.StopAsync();
        await dbContainer.DisposeAsync();

        await base.DisposeAsync(); // Respecting existing implementation.
    }

    public HttpClient GetAuthorizedClient(TestUser testUser)
    {
        var jwtUser = testUser.ToJwtUser(DateTimeOffset.UtcNow, TimeSpan.FromDays(1));
        var cookieJwtToken = jwtFactory.CreateJwtToken(jwtUser);
        var httpClient = this.CreateDefaultClient();
        httpClient.DefaultRequestHeaders.Add("Cookie", $"{ApplicationConstants.AccessCookieName}={cookieJwtToken}");

        return httpClient;
    }
}