using System.Security.Cryptography;
using System.Text;
using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;

namespace Web.Services;

public class AudioStatisticsService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AudioStatisticsService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task TrackAsync(
        int audioFileId,
        AudioStatisticEventType eventType,
        HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        var ipHash = HashIpAddress(ipAddress);

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        var statistic = new AudioStatistic
        {
            AudioFileId = audioFileId,
            EventType = eventType,
            CreatedAt = DateTime.UtcNow,
            IpHash = ipHash,
            UserAgent = string.IsNullOrWhiteSpace(userAgent)
                ? null
                : userAgent
        };

        _context.AudioStatistics.Add(statistic);

        await _context.SaveChangesAsync();
    }

    private string? HashIpAddress(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        var salt = _configuration["Statistics:IpHashSalt"];

        if (string.IsNullOrWhiteSpace(salt))
            salt = "MajalesAhvaz-Statistics";

        var input = $"{salt}:{ipAddress}";

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes);
    }
}