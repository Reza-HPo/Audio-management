namespace Web.Areas.Admin.Models.SiteContent;

public class HomeContentViewModel
{
    // =========================================================
    // HERO
    // =========================================================

    public string HeroTitle { get; set; } = string.Empty;

    public string? HeroSubtitle { get; set; }

    public string? HeroDescription { get; set; }

    public string? HeroButtonText { get; set; }

    public string? HeroButtonUrl { get; set; }

    public string? HeroImageUrl { get; set; }


    // =========================================================
    // INTRODUCTION
    // =========================================================

    public string? IntroTitle { get; set; }

    public string? IntroText { get; set; }


    // =========================================================
    // AUDIO ARCHIVE
    // =========================================================

    public string? ArchiveTitle { get; set; }

    public string? ArchiveDescription { get; set; }

    public string? ArchiveButtonText { get; set; }

    public string? ArchiveButtonUrl { get; set; }


    // =========================================================
    // SPEAKERS
    // =========================================================

    public string? SpeakersTitle { get; set; }

    public string? SpeakersDescription { get; set; }


    // =========================================================
    // FOOTER
    // =========================================================

    public string? FooterDescription { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }


    // =========================================================
    // SOCIAL MEDIA
    // =========================================================

    public string? BaleUrl { get; set; }

    public string? EitaaUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? TelegramUrl { get; set; }
}