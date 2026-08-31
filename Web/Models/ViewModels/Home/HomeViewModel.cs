namespace Web.Models.ViewModels.Home;

public class HomeViewModel
{
    // =========================================================
    // Site Content
    // =========================================================

    public HomeContentViewModel Content { get; set; } = new();


    // =========================================================
    // Statistics
    // =========================================================

    public int AudioCount { get; set; }

    public int SpeakerCount { get; set; }

    public int CategoryCount { get; set; }


    // =========================================================
    // Latest Audios
    // =========================================================

    public List<HomeAudioViewModel> LatestAudios { get; set; } = [];


    // =========================================================
    // Categories
    // =========================================================

    public List<HomeCategoryViewModel> Categories { get; set; } = [];


    // =========================================================
    // Speakers
    // =========================================================

    public List<HomeSpeakerViewModel> Speakers { get; set; } = [];
}


// =============================================================
// HOME CONTENT
// =============================================================

public class HomeContentViewModel
{
    // ---------------------------------------------------------
    // Hero
    // ---------------------------------------------------------

    public string? HeroTitle { get; set; }

    public string? HeroSubtitle { get; set; }

    public string? HeroDescription { get; set; }

    public string? HeroButtonText { get; set; }

    public string? HeroButtonUrl { get; set; }

    public string? HeroImageUrl { get; set; }


    // ---------------------------------------------------------
    // Introduction
    // ---------------------------------------------------------

    public string? IntroTitle { get; set; }

    public string? IntroText { get; set; }


    // ---------------------------------------------------------
    // Archive
    // ---------------------------------------------------------

    public string? ArchiveTitle { get; set; }

    public string? ArchiveDescription { get; set; }

    public string? ArchiveButtonText { get; set; }

    public string? ArchiveButtonUrl { get; set; }


    // ---------------------------------------------------------
    // Speakers
    // ---------------------------------------------------------

    public string? SpeakersTitle { get; set; }

    public string? SpeakersDescription { get; set; }

    // Footer

    public string? FooterDescription { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }


    // Social

    public string? BaleUrl { get; set; }

    public string? EitaaUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? TelegramUrl { get; set; }
}


// =============================================================
// HOME AUDIO
// =============================================================

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


// =============================================================
// HOME CATEGORY
// =============================================================

public class HomeCategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AudioCount { get; set; }
}


// =============================================================
// HOME SPEAKER
// =============================================================

public class HomeSpeakerViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int AudioCount { get; set; }
}