using System.Diagnostics;
using App;
using App.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterTodoDependencies();

var app = builder.Build();

app.MapOpenApiAndScalar();

app.MapControllers();

app.LogStartup();

await using (var scope = app.Services.CreateAsyncScope())
{
    var activity = new Activity("Program");
    Activity.Current = activity;
    activity.Start();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Hello, World!");
}

app.Run();