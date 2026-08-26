namespace Web.Models.ViewModels.Archive;

public class ArchiveViewModel
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public int? SpeakerId { get; set; }

    public string Sort { get; set; } = "newest";

    public List<AudioItemViewModel> Audios { get; set; } = [];

    public List<CategoryFilterViewModel> Categories { get; set; } = [];

    public List<SpeakerFilterViewModel> Speakers { get; set; } = [];

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }
}

public class AudioItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? SpeakerName { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? FileName { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsDownloadable { get; set; }

    public List<string> Categories { get; set; } = [];
}

public class CategoryFilterViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class SpeakerFilterViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}