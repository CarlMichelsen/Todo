using System.Collections.ObjectModel;
using Presentation.Configuration;

namespace Application.Configuration;

public class CorsOptions : IConfigurationOptions
{
    public static string SectionName => "Cors";
    
    public required Collection<Uri> AllowedDomains { get; init; }
}