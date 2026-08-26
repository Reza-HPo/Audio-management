namespace Web.Models.ViewModels.Audio;

public class AudioDetailsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? FileUrl { get; set; }

    public string? CoverImageUrl { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? SpeakerName { get; set; }

    public int? SpeakerId { get; set; }

    public string? SpeakerImageUrl { get; set; }

    public string? SpeakerBio { get; set; }

    public bool IsDownloadable { get; set; }

    public List<AudioCategoryViewModel> Categories { get; set; } = [];
}


public class AudioCategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}