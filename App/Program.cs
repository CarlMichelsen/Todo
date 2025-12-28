using App;
using App.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterTodoDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseGlobalExceptionHandler(app.Logger);

app.MapHealthChecks("health");

app.UseCors();

app.UseStaticFiles(StaticFileOptionsFactory.Create());

app.MapFallbackToFile("index.html");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await using var scope = app.Services.CreateAsyncScope();
var calendar = await scope.TestIcsClient();

Console.WriteLine(calendar?.Id);
await app.RunAsync();