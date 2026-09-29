using Microsoft.AspNetCore.Http;

namespace Web.Services;

public class VisitorIdentityService
{
    public const string CookieName = "majales_visitor_id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public VisitorIdentityService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetOrCreateVisitorId()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException(
                "HttpContext is not available.");

        if (httpContext.Request.Cookies.TryGetValue(
                CookieName,
                out var existingVisitorId)
            && !string.IsNullOrWhiteSpace(existingVisitorId))
        {
            return existingVisitorId;
        }

        var visitorId = Guid.NewGuid().ToString("N");

        httpContext.Response.Cookies.Append(
            CookieName,
            visitorId,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                MaxAge = TimeSpan.FromDays(365)
            });

        return visitorId;
    }
}