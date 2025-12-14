using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Presentation.Attribute;

/// <summary>
/// Validates that a string is a valid hexadecimal color code.
/// Supports both 3-digit (#RGB) and 6-digit (#RRGGBB) formats with optional hash prefix.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public partial class HexColorAttribute : ValidationAttribute
{
    private readonly bool requireHash;
    
    // Regex pattern for hex colors: optional #, then 3 or 6 hex digits
    private const string ColorPattern = @"^#?([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$";
    
    [GeneratedRegex(ColorPattern)]
    private static partial Regex ColorRegex();
    
    /// <summary>
    /// Initializes a new instance of the HexColorAttribute class.
    /// </summary>
    /// <param name="requireHash">If true, the hash (#) prefix is required. Default is false.</param>
    public HexColorAttribute(bool requireHash = false)
    {
        this.requireHash = requireHash;
        ErrorMessage ??= requireHash 
            ? "The {0} field must be a valid hex color with # prefix (e.g., #FF5733 or #ABC)."
            : "The {0} field must be a valid hex color (e.g., #FF5733, FF5733, #ABC, or ABC).";
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Null or empty strings are considered valid (use [Required] for mandatory fields)
        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            return ValidationResult.Success;
        }
        
        if (value is not string colorString)
        {
            return new ValidationResult($"The {validationContext.DisplayName} field must be a string.");
        }
        
        var trimmed = colorString.Trim();
        
        // Check if hash is required but missing
        if (requireHash && !trimmed.StartsWith('#'))
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                new[] { validationContext.MemberName ?? string.Empty });
        }
        
        // Validate with regex
        if (!ColorRegex().IsMatch(trimmed))
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                new[] { validationContext.MemberName ?? string.Empty });
        }
        
        return ValidationResult.Success;
    }
}