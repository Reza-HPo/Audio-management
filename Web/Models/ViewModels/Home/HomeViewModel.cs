namespace Web.Models.ViewModels.Home;

public class HomeViewModel
{
    public int AudioCount { get; set; }

    public int SpeakerCount { get; set; }

    public int CategoryCount { get; set; }

    public List<HomeAudioViewModel> LatestAudios { get; set; } = [];

    public List<HomeCategoryViewModel> Categories { get; set; } = [];

    public List<HomeSpeakerViewModel> Speakers { get; set; } = [];
}


public class HomeAudioViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? SpeakerName { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? FileName { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? PublishedAt { get; set; }

    public List<string> Categories { get; set; } = [];
}


public class HomeCategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AudioCount { get; set; }
}


public class HomeSpeakerViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int AudioCount { get; set; }
}