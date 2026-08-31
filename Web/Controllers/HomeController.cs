using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.ViewModels.Home;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // -----------------------------------------------------
        // Audio Query
        // -----------------------------------------------------

        var audioQuery = _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished);


        // -----------------------------------------------------
        // Site Settings
        // -----------------------------------------------------

        var settings = await _context.SiteSettings
            .AsNoTracking()
            .Where(x =>
                x.Group == "Home" ||
                x.Group == "Footer")
            .ToListAsync();


        // -----------------------------------------------------
        // Home Model
        // -----------------------------------------------------

        var model = new HomeViewModel
        {
            // =================================================
            // CONTENT
            // =================================================

            Content = new HomeContentViewModel
            {
                // -------------------------------------------------
                // Hero
                // -------------------------------------------------

                HeroTitle = GetValue(
                    settings,
                    "Home.HeroTitle"),

                HeroSubtitle = GetValue(
                    settings,
                    "Home.HeroSubtitle"),

                HeroDescription = GetValue(
                    settings,
                    "Home.HeroDescription"),

                HeroButtonText = GetValue(
                    settings,
                    "Home.HeroButtonText"),

                HeroButtonUrl = GetValue(
                    settings,
                    "Home.HeroButtonUrl"),

                HeroImageUrl = GetValue(
                    settings,
                    "Home.HeroImageUrl"),


                // -------------------------------------------------
                // Introduction
                // -------------------------------------------------

                IntroTitle = GetValue(
                    settings,
                    "Home.IntroTitle"),

                IntroText = GetValue(
                    settings,
                    "Home.IntroText"),


                // -------------------------------------------------
                // Archive
                // -------------------------------------------------

                ArchiveTitle = GetValue(
                    settings,
                    "Home.ArchiveTitle"),

                ArchiveDescription = GetValue(
                    settings,
                    "Home.ArchiveDescription"),

                ArchiveButtonText = GetValue(
                    settings,
                    "Home.ArchiveButtonText"),

                ArchiveButtonUrl = GetValue(
                    settings,
                    "Home.ArchiveButtonUrl"),


                // -------------------------------------------------
                // Speakers
                // -------------------------------------------------

                SpeakersTitle = GetValue(
                    settings,
                    "Home.SpeakersTitle"),

                SpeakersDescription = GetValue(
                    settings,
                    "Home.SpeakersDescription"),


                // -------------------------------------------------
                // Footer
                // -------------------------------------------------

                FooterDescription = GetValue(
                    settings,
                    "Footer.Description"),

                Phone = GetValue(
                    settings,
                    "Footer.Phone"),

                Email = GetValue(
                    settings,
                    "Footer.Email"),

                Address = GetValue(
                    settings,
                    "Footer.Address"),


                // -------------------------------------------------
                // Social Media
                // -------------------------------------------------

                BaleUrl = GetValue(
                    settings,
                    "Footer.BaleUrl"),

                EitaaUrl = GetValue(
                    settings,
                    "Footer.EitaaUrl"),

                InstagramUrl = GetValue(
                    settings,
                    "Footer.InstagramUrl"),

                TelegramUrl = GetValue(
                    settings,
                    "Footer.TelegramUrl")
            },


            // =================================================
            // STATISTICS
            // =================================================

            AudioCount = await audioQuery.CountAsync(),

            SpeakerCount = await _context.Speakers
                .AsNoTracking()
                .CountAsync(s => s.IsActive),

            CategoryCount = await _context.Categories
                .AsNoTracking()
                .CountAsync(c => c.IsActive),


            // =================================================
            // LATEST AUDIOS
            // =================================================

            LatestAudios = await audioQuery
                .Include(a => a.Speaker)
                .Include(a => a.AudioCategories)
                    .ThenInclude(ac => ac.Category)
                .OrderByDescending(a =>
                    a.PublishedAt ?? a.CreatedAt)
                .Take(6)
                .Select(a => new HomeAudioViewModel
                {
                    Id = a.Id,

                    Title = a.Title,

                    SpeakerName = a.Speaker != null
                        ? a.Speaker.Name
                        : null,

                    CoverImageUrl = a.CoverImageUrl,

                    FileName = a.FileName,

                    Duration = a.Duration,

                    PublishedAt = a.PublishedAt,

                    Categories = a.AudioCategories
                        .Where(ac => ac.Category != null)
                        .Select(ac => ac.Category.Name)
                        .ToList()
                })
                .ToListAsync(),


            // =================================================
            // CATEGORIES
            // =================================================

            Categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Select(c => new HomeCategoryViewModel
                {
                    Id = c.Id,

                    Name = c.Name,

                    AudioCount = c.AudioCategories
                        .Count(ac =>
                            ac.AudioFile.IsPublished)
                })
                .Take(8)
                .ToListAsync(),


            // =================================================
            // SPEAKERS
            // =================================================

            Speakers = await _context.Speakers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Select(s => new HomeSpeakerViewModel
                {
                    Id = s.Id,

                    Name = s.Name,

                    ImageUrl = s.ImageUrl,

                    AudioCount = _context.AudioFiles
                        .Count(a =>
                            a.IsPublished &&
                            a.SpeakerId == s.Id)
                })
                .OrderByDescending(s => s.AudioCount)
                .ThenBy(s => s.Name)
                .Take(4)
                .ToListAsync()
        };


        return View(model);
    }


    // =========================================================
    // GET SETTING VALUE
    // =========================================================

    private static string? GetValue(
        List<MaktabAhvaz.Domain.Entities.SiteSetting> settings,
        string key)
    {
        return settings
            .FirstOrDefault(x => x.Key == key)
            ?.Value;
    }
}