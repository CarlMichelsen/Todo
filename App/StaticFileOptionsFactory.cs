namespace App;

public static class StaticFileOptionsFactory
{
    public static StaticFileOptions Create()
    {
        return new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            DefaultContentType = "application/octet-stream",
            OnPrepareResponse = context =>
            {
                const int oneWeek = 604800;
                context.Context.Response.Headers.Append("Cache-Control", $"public, max-age={oneWeek * 53}");
            },
        };
    }
}