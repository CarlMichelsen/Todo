using System.Text;
using Application.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App;

public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters AccessValidationParameters(
        JwtOptions jwtOptions,
        TimeProvider? inputTimeProvider = null)
    {
        var timeProvider = inputTimeProvider ?? TimeProvider.System;
        
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Crucial for lifetime validation
            ClockSkew = TimeSpan.Zero, // Crucial for lifetime validation
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKeys = jwtOptions.Secrets
                .Select(key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))),
            // Use TimeProvider for lifetime validation
            LifetimeValidator = (notBefore, expires, _, _) =>
            {
                var now = timeProvider.GetUtcNow().UtcDateTime;
                
                if (notBefore.HasValue && now < notBefore.Value)
                    return false;
                    
                if (expires.HasValue && now > expires.Value)
                    return false;
                    
                return true;
            }
        };
    }
}