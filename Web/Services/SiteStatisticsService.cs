using System.Security.Cryptography;
using System.Text;
using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;

namespace Web.Services;

public class SiteStatisticsService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public SiteStatisticsService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task TrackAsync(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value;

        if (string.IsNullOrWhiteSpace(path))
            path = "/";

        var ipAddress =
            httpContext.Connection.RemoteIpAddress?.ToString();

        var statistic = new SiteStatistic
        {
            EventType = SiteStatisticEventType.PageView,
            Path = path,
            CreatedAt = DateTime.UtcNow,
            IpHash = HashIpAddress(ipAddress),
            UserAgent = GetUserAgent(httpContext)
        };

        _context.SiteStatistics.Add(statistic);

        await _context.SaveChangesAsync();
    }

    private string? GetUserAgent(HttpContext httpContext)
    {
        var userAgent =
            httpContext.Request.Headers.UserAgent.ToString();

        return string.IsNullOrWhiteSpace(userAgent)
            ? null
            : userAgent;
    }

    private string? HashIpAddress(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        var salt =
            _configuration["Statistics:IpHashSalt"];

        if (string.IsNullOrWhiteSpace(salt))
            salt = "MajalesAhvaz-Statistics";

        var input = $"{salt}:{ipAddress}";

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes);
    }
}