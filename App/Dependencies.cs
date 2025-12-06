using System.Text.Json.Serialization;
using App.Extensions;
using Application.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Presentation;

namespace App;

public static class Dependencies
{
    public static void RegisterTodoDependencies(
        this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration
            .AddJsonFile(
                "secrets.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(Program).Assembly)
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Environment.ApplicationName = ApplicationConstants.Name;

        // Utility 
        builder
            .ApplicationUseSerilog()
            .Services
            .AddSingleton(TimeProvider.System)
            .AddHttpContextAccessor();
        
        // Auth
        builder.RegisterAuthDependencies();
        
        // Options
        builder.Services
            .AddConfigurationOptions<JwtOptions>(builder.Configuration)
            .AddConfigurationOptions<CorsOptions>(builder.Configuration);
        
        // CORS
        var corsConfig = builder
            .Configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>() ?? throw new NullReferenceException("Cors options not found");
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var origins = corsConfig
                    .AllowedDomains
                    .Select(uri => uri.GetLeftPart(UriPartial.Authority).TrimEnd('/'))
                    .ToArray();
                policy
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
    }
    
    private static void RegisterAuthDependencies(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? throw new NullReferenceException($"Failed to get {nameof(JwtOptions)} during startup");
        
        builder.Services
            .AddAuthentication()
            .AddJwtBearer("CookieScheme", options =>
            {
                var timeProvider = builder.Services.BuildServiceProvider()
                    .GetService<TimeProvider>();
            
            // Configure JWT settings
            options.TokenValidationParameters = TokenValidationParametersFactory
                .AccessValidationParameters(jwtOptions, timeProvider);
            
            options.MapInboundClaims = false; // Important!

            // Get token from cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[ApplicationConstants.AccessCookieName];
                    return Task.CompletedTask;
                },
            };
        });
        
        builder.Services
            .AddAuthorization();
    }
}