using MaktabAhvaz.Domain.Entities;

namespace Web.Areas.Admin.Models.AudioFiles;

public class AudioFileListViewModel
{
    public List<AudioFile> Items { get; set; } = new();

    // Filters
    public string? Search { get; set; }

    public int? SpeakerId { get; set; }

    public int? CategoryId { get; set; }

    public string? Status { get; set; }

    public string? Downloadable { get; set; }

    public string Sort { get; set; } = "latest";

    // Pagination
    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalItems { get; set; }

    public int TotalPages =>
        PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalItems / (double)PageSize);

    // Filter options
    public List<Speaker> Speakers { get; set; } = new();

    public List<Category> Categories { get; set; } = new();

    // Statistics
    public int TotalAudioCount { get; set; }

    public int PublishedAudioCount { get; set; }

    public int DraftAudioCount { get; set; }

    public long TotalFileSize { get; set; }
}
