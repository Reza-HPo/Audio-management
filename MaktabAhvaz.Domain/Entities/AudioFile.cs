namespace MaktabAhvaz.Domain.Entities;

public class AudioFile
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public TimeSpan? Duration { get; set; }

    public long FileSize { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = false;

    public bool IsDownloadable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    // Speaker
    public int SpeakerId { get; set; }

    public Speaker Speaker { get; set; } = null!;

    // Categories
    public ICollection<AudioCategory> AudioCategories { get; set; }
        = new List<AudioCategory>();
}