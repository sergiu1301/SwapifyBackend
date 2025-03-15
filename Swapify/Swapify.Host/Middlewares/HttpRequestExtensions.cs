using Validation;

namespace Swapify.Host.Middlewares;

public static class HttpRequestExtensions
{
    public static bool IsApiCall(this HttpRequest httpRequest)
    {
        Requires.NotNull(httpRequest, nameof(httpRequest));

        string currentPath = httpRequest.Path;

        bool isApiPath = currentPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);

        return isApiPath;
    }

    public static bool IsAuthEndpoint(this HttpRequest httpRequest)
    {
        Requires.NotNull(httpRequest, nameof(httpRequest));

        string currentPath = httpRequest.Path;

        bool isIdentityServerEndpoint = currentPath.StartsWith("/connect/", StringComparison.OrdinalIgnoreCase);

        return isIdentityServerEndpoint;
    }
}