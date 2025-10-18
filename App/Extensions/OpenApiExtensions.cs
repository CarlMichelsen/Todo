using Scalar.AspNetCore;

namespace App.Extensions;

public static class OpenApiExtensions
{
    public static WebApplication MapOpenApiAndScalar(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}
