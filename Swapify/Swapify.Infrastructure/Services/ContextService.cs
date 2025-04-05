using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Swapify.Contracts.Services;
using Swapify.Infrastructure.Extensions;

namespace Swapify.Infrastructure.Services;

public class ContextService: IContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> GetCurrentUserIdAsync()
    {
        string? userId = GetClaimValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return null;
        }

        return userId;
    }

    private string? GetClaimValue(string claim)
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        return user?.Claims.GetClaimValue(claim);
    }
}