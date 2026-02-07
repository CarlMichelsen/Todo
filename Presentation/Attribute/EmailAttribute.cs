using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Presentation.Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed partial class EmailAttribute : ValidationAttribute
{
    private static readonly Regex EmailRegex = CompiledEmailRegex();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null or "")
            return ValidationResult.Success; // Use [Required] for null checks

        if (value is not string email)
            return new ValidationResult("Invalid type");

        return EmailRegex.IsMatch(email)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? "Invalid email address");
    }

    [GeneratedRegex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled,
        "en-DK"
    )]
    private static partial Regex CompiledEmailRegex();
}
