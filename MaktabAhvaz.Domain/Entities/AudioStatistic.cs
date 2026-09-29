namespace MaktabAhvaz.Domain.Entities;

public class AudioStatistic
{
    public long Id { get; set; }

    public int AudioFileId { get; set; }

    public AudioFile AudioFile { get; set; } = null!;

    public AudioStatisticEventType EventType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// شناسه ناشناس و یکتای بازدیدکننده
    /// </summary>
    public string? VisitorId { get; set; }

    /// <summary>
    /// Hash شده IP بازدیدکننده
    /// </summary>
    public string? IpHash { get; set; }

    public string? UserAgent { get; set; }
}