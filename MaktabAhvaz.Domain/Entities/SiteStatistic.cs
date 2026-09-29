namespace MaktabAhvaz.Domain.Entities;

public class SiteStatistic
{
    public long Id { get; set; }

    public SiteStatisticEventType EventType { get; set; }

    public string Path { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// شناسه ناشناس و یکتای مرورگر/بازدیدکننده
    /// </summary>
    public string? VisitorId { get; set; }

    /// <summary>
    /// Hash شده IP بازدیدکننده
    /// </summary>
    public string? IpHash { get; set; }

    public string? UserAgent { get; set; }
}