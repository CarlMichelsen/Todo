using App;
using App.Extensions;
using Application;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterTodoDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGlobalExceptionHandler(app.Logger);

app.UseStaticFiles(StaticFileOptionsFactory.Create());

app.MapFallbackToFile("index.html");

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var logger = context
        .RequestServices
        .GetRequiredService<ILogger<Program>>();
    try
    {
        var jwtUser = context.GetJwtUser();
        logger.LogInformation(
            "'{Username}' - {UserId}",
            jwtUser.Username,
            jwtUser.UserId);
    }
    catch (Exception e)
    {
        logger.LogInformation(
            e,
            "Unable to authenticate user");
    }
    
    await next(context);
});

app.Run();