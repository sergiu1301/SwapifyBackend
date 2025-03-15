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

    public async Task<string> GetCurrentUserIdAsync()
    {
        string? userId = GetClaimValue("user_id");

        if (userId is null)
        {
            return null;
        }

        return userId;
    }

    public async Task<string> GetCurrentContextAsync()
    {
        string? userEmail = GetClaimValue(ClaimTypes.Email);

        if (userEmail is null)
        {
            return null;
        }

        return userEmail;
    }

    public async Task<string> GetCurrentGoogleFirstNameAsync()
    {
        string? first_name = GetClaimValue(ClaimTypes.Surname);
        if (first_name is null)
        {
            return null;
        }

        return first_name;
    }

    public async Task<string> GetCurrentGoogleLastNameAsync()
    {
        string? last_name = GetClaimValue(ClaimTypes.GivenName);
        if (last_name is null)
        {
            return null;
        }

        return last_name;
    }

    private string? GetClaimValue(string claim)
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        return user?.Claims.GetClaimValue(claim);
    }
}