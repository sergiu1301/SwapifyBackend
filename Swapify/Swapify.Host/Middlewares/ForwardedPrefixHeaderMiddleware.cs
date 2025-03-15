using Microsoft.Extensions.Primitives;
using Validation;

namespace Swapify.Host.Middlewares
{
    public class ForwardedPrefixHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ForwardedPrefixHeaderMiddleware> _logger;

        public ForwardedPrefixHeaderMiddleware(
            RequestDelegate next,
            ILogger<ForwardedPrefixHeaderMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Requires.NotNull(context, nameof(context));

            if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out StringValues pathBase))
            {
                string url = context.Request.Path;
                string verb = context.Request.Method;

                _logger.LogDebug("Request {Verb} {Url} using X-Forwarded-Prefix: {XForwardedPrefix}", verb, url, pathBase);

                context.Request.PathBase = pathBase.Last();

                if (context.Request.Path.StartsWithSegments(context.Request.PathBase, out PathString path))
                {
                    context.Request.Path = path;
                }
            }

            await _next(context);
        }
    }
}
