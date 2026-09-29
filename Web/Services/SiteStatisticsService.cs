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

    public async Task TrackPageViewAsync(
        string path,
        string visitorId,
        string? ipAddress,
        string? userAgent)
    {
        var statistic = new SiteStatistic
        {
            EventType = SiteStatisticEventType.PageView,
            Path = path,
            CreatedAt = DateTime.UtcNow,
            VisitorId = visitorId,
            IpHash = HashIp(ipAddress),
            UserAgent = userAgent
        };

        _context.SiteStatistics.Add(statistic);

        await _context.SaveChangesAsync();
    }

    private string? HashIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        var salt = _configuration["Statistics:IpHashSalt"];

        if (string.IsNullOrWhiteSpace(salt))
            salt = "majales-ahvaz-statistics";

        var input = $"{salt}:{ipAddress}";

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes);
    }
}