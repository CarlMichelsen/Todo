using System.Collections.ObjectModel;
using Presentation.Configuration;

namespace Application.Configuration;

public class JwtOptions : IConfigurationOptions
{
    public static string SectionName => "Jwt";
    
    // ReSharper disable once CollectionNeverUpdated.Global
    public required Collection<string> Secrets { get; init; }

    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
}