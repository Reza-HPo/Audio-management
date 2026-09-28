using Web.Services;

namespace Web.Middleware;

public class SiteStatisticsMiddleware
{
    private readonly RequestDelegate _next;

    public SiteStatisticsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SiteStatisticsService statisticsService)
    {
        if (!ShouldTrack(context))
        {
            await _next(context);
            return;
        }

        await _next(context);

        if (context.Response.StatusCode != StatusCodes.Status200OK)
            return;

        var contentType = context.Response.ContentType;

        if (string.IsNullOrWhiteSpace(contentType) ||
            !contentType.StartsWith(
                "text/html",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await statisticsService.TrackAsync(context);
    }

    private static bool ShouldTrack(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
            return false;

        var path = context.Request.Path.Value;

        if (string.IsNullOrWhiteSpace(path))
            return false;

        path = path.ToLowerInvariant();

        // API
        if (path.StartsWith("/api"))
            return false;

        // Identity / Authentication
        if (path.StartsWith("/identity"))
            return false;

        // Admin
        if (path.StartsWith("/admin"))
            return false;

        // Static files
        if (HasStaticFileExtension(path))
            return false;

        return true;
    }

    private static bool HasStaticFileExtension(string path)
    {
        var extensions = new[]
        {
            ".css",
            ".js",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".webp",
            ".svg",
            ".ico",
            ".woff",
            ".woff2",
            ".ttf",
            ".eot",
            ".mp3",
            ".wav",
            ".mp4",
            ".webm",
            ".pdf",
            ".zip",
            ".rar",
            ".xml",
            ".json",
            ".txt"
        };

        return extensions.Any(path.EndsWith);
    }
}