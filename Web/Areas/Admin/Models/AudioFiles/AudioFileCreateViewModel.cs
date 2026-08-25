using Microsoft.AspNetCore.Http;

namespace Web.Areas.Admin.Models.AudioFiles;

public class AudioFileCreateViewModel
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public IFormFile? Audio { get; set; }

    public IFormFile? CoverImage { get; set; }

    public int SpeakerId { get; set; }

    public List<int> CategoryIds { get; set; } = [];

    public bool IsPublished { get; set; }

    public bool IsDownloadable { get; set; } = true;
}