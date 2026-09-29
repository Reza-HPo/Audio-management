using System.Security.Cryptography;
using System.Text;
using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;

namespace Web.Services;

public class AudioStatisticsService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly VisitorIdentityService _visitorIdentityService;

    public AudioStatisticsService(
        ApplicationDbContext context,
        IConfiguration configuration,
        VisitorIdentityService visitorIdentityService)
    {
        _context = context;
        _configuration = configuration;
        _visitorIdentityService = visitorIdentityService;
    }

    public async Task TrackViewAsync(
        int audioFileId,
        string? ipAddress,
        string? userAgent)
    {
        await TrackAsync(
            audioFileId,
            AudioStatisticEventType.View,
            ipAddress,
            userAgent);
    }

    public async Task TrackDownloadAsync(
        int audioFileId,
        string? ipAddress,
        string? userAgent)
    {
        await TrackAsync(
            audioFileId,
            AudioStatisticEventType.Download,
            ipAddress,
            userAgent);
    }

    private async Task TrackAsync(
        int audioFileId,
        AudioStatisticEventType eventType,
        string? ipAddress,
        string? userAgent)
    {
        var visitorId =
            _visitorIdentityService.GetOrCreateVisitorId();

        var statistic = new AudioStatistic
        {
            AudioFileId = audioFileId,
            EventType = eventType,
            CreatedAt = DateTime.UtcNow,
            VisitorId = visitorId,
            IpHash = HashIp(ipAddress),
            UserAgent = userAgent
        };

        _context.AudioStatistics.Add(statistic);

        await _context.SaveChangesAsync();
    }

    private string? HashIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        var salt =
            _configuration["Statistics:IpHashSalt"];

        if (string.IsNullOrWhiteSpace(salt))
            salt = "majales-ahvaz-statistics";

        var input = $"{salt}:{ipAddress}";

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes);
    }
}