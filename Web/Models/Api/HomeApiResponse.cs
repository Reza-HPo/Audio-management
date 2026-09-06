namespace Web.Models.Api;

public class HomeApiResponse
{
    public List<HomeAudioDto> LatestAudios { get; set; } = [];
    public List<HomeSpeakerDto> Speakers { get; set; } = [];
    public List<HomeCategoryDto> Categories { get; set; } = [];
}

public class HomeAudioDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? PublishedAt { get; set; }
    public HomeSpeakerDto? Speaker { get; set; }
}

public class HomeSpeakerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class HomeCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}