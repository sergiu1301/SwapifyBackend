using System.Security.Claims;
using Validation;

namespace Swapify.Infrastructure.Extensions;

public static class ClaimExtensions
{
    public static string GetClaimValue(this IEnumerable<Claim> claims, string claimType)
    {
        Requires.NotNullOrWhiteSpace(claimType, nameof(claimType));

        Claim? claim = claims.FirstOrDefault(c => string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase));

        return claim!.Value;
    }
}