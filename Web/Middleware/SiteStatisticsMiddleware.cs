using Microsoft.AspNetCore.Http;
using Web.Services;

namespace Web.Middleware;

public class SiteStatisticsMiddleware
{
    private readonly RequestDelegate _next;

    public SiteStatisticsMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SiteStatisticsService statisticsService,
        VisitorIdentityService visitorIdentityService)
    {
        if (!ShouldTrack(context))
        {
            await _next(context);
            return;
        }

        var visitorId =
            visitorIdentityService.GetOrCreateVisitorId();

        await _next(context);

        if (context.Response.StatusCode != StatusCodes.Status200OK)
            return;

        var contentType =
            context.Response.ContentType;

        if (string.IsNullOrWhiteSpace(contentType) ||
            !contentType.StartsWith(
                "text/html",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var path = context.Request.Path.Value;

        if (string.IsNullOrWhiteSpace(path))
            return;

        await statisticsService.TrackPageViewAsync(
            path,
            visitorId,
            context.Connection.RemoteIpAddress?.ToString(),
            context.Request.Headers.UserAgent.ToString());
    }

    private static bool ShouldTrack(
        HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
            return false;

        var path = context.Request.Path.Value;

        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (path.StartsWith(
                "/api",
                StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.StartsWith(
                "/identity",
                StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.StartsWith(
                "/admin",
                StringComparison.OrdinalIgnoreCase))
            return false;

        if (IsStaticFile(path))
            return false;

        return true;
    }

    private static bool IsStaticFile(string path)
    {
        var extensions = new[]
        {
            ".css",
            ".js",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".svg",
            ".webp",
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
            ".zip"
        };

        return extensions.Any(
            extension =>
                path.EndsWith(
                    extension,
                    StringComparison.OrdinalIgnoreCase));
    }
}