namespace Database.Entity.Value;

public sealed class EmailValue : IEquatable<EmailValue>
{
    public string Value { get; }

    private EmailValue(string value)
    {
        Value = value;
    }

    public static EmailValue Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new EmailValueException("Email cannot be empty");
        }

        if (email.Length > 254)
        {
            throw new EmailValueException("Email is too long");
        }

        return IsValidEmail(email)
            ? new EmailValue(email.ToUpperInvariant())
            : throw new EmailValueException("Email format is invalid");
    }

    public static bool IsValidEmail(string email)
    {
        var emailRegex = new System.Text.RegularExpressions.Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
        return emailRegex.IsMatch(email);
    }

    public static implicit operator string(EmailValue email)
    {
        return email.Value;
    }

    public bool Equals(EmailValue? other)
    {
        return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is EmailValue other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;

    public static bool operator ==(EmailValue? left, EmailValue? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(EmailValue? left, EmailValue? right) => !(left == right);
}

public sealed class EmailValueException : Exception
{
    public EmailValueException() { }

    public EmailValueException(string message)
        : base(message) { }

    public EmailValueException(string message, Exception innerException)
        : base(message, innerException) { }
}
