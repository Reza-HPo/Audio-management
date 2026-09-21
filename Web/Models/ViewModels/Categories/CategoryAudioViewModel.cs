namespace Web.Models.Categories;

public class CategoryAudioViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? FileName { get; set; }

    public string? CoverImageUrl { get; set; }

    public TimeSpan? Duration { get; set; }

    public long? FileSize { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int SpeakerId { get; set; }

    public string SpeakerName { get; set; } = string.Empty;

    public string? SpeakerImageUrl { get; set; }
}