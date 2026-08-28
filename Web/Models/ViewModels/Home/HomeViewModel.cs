namespace Web.Models.ViewModels.Home;

public class HomeViewModel
{
    public HomeContentViewModel Content { get; set; } = new();

    public int AudioCount { get; set; }

    public int SpeakerCount { get; set; }

    public int CategoryCount { get; set; }

    public List<HomeAudioViewModel> LatestAudios { get; set; } = [];

    public List<HomeCategoryViewModel> Categories { get; set; } = [];

    public List<HomeSpeakerViewModel> Speakers { get; set; } = [];
}


public class HomeContentViewModel
{
    public string? HeroTitle { get; set; }

    public string? HeroSubtitle { get; set; }

    public string? HeroDescription { get; set; }

    public string? HeroButtonText { get; set; }

    public string? HeroButtonUrl { get; set; }

    public string? HeroImageUrl { get; set; }

    public string? IntroTitle { get; set; }

    public string? IntroText { get; set; }

    public string? ArchiveTitle { get; set; }

    public string? ArchiveDescription { get; set; }

    public string? ArchiveButtonText { get; set; }

    public string? ArchiveButtonUrl { get; set; }

    public string? SpeakersTitle { get; set; }

    public string? SpeakersDescription { get; set; }
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