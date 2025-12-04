using Presentation.Configuration;

namespace Application.Configuration;

public class CorsOptions : IConfigurationOptions
{
    public static string SectionName => "Cors";
    
    public required List<Uri> AllowedDomains { get; init; }
}