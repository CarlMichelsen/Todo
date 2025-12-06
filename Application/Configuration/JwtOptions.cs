using Presentation.Configuration;

namespace Application.Configuration;

public class JwtOptions : IConfigurationOptions
{
    public static string SectionName => "Jwt";
    
    public required List<string> Secrets { get; init; }

    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
}