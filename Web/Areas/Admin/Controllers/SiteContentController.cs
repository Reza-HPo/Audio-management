using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Models.SiteContent;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SiteContentController : Controller
{
    private readonly ApplicationDbContext _context;

    public SiteContentController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // HOME - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Home()
    {
        var settings = await _context.SiteSettings
            .AsNoTracking()
            .Where(x => x.Group == "Home" || x.Group == "Footer")
            .ToListAsync();

        var model = new HomeContentViewModel
        {
            // -------------------------------------------------
            // Hero
            // -------------------------------------------------

            HeroTitle = GetValue(settings, "Home.HeroTitle"),

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
        };

        return View(model);
    }


    // =========================================================
    // HOME - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Home(
        HomeContentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // -----------------------------------------------------
        // Home
        // -----------------------------------------------------

        await SaveSetting(
            "Home.HeroTitle",
            model.HeroTitle,
            "Home",
            "عنوان اصلی Hero صفحه اصلی");

        await SaveSetting(
            "Home.HeroSubtitle",
            model.HeroSubtitle,
            "Home",
            "زیرعنوان Hero");

        await SaveSetting(
            "Home.HeroDescription",
            model.HeroDescription,
            "Home",
            "توضیحات Hero");

        await SaveSetting(
            "Home.HeroButtonText",
            model.HeroButtonText,
            "Home",
            "متن دکمه Hero");

        await SaveSetting(
            "Home.HeroButtonUrl",
            model.HeroButtonUrl,
            "Home",
            "لینک دکمه Hero");

        await SaveSetting(
            "Home.HeroImageUrl",
            model.HeroImageUrl,
            "Home",
            "تصویر Hero");


        // -----------------------------------------------------
        // Introduction
        // -----------------------------------------------------

        await SaveSetting(
            "Home.IntroTitle",
            model.IntroTitle,
            "Home",
            "عنوان بخش معرفی");

        await SaveSetting(
            "Home.IntroText",
            model.IntroText,
            "Home",
            "متن بخش معرفی");


        // -----------------------------------------------------
        // Archive
        // -----------------------------------------------------

        await SaveSetting(
            "Home.ArchiveTitle",
            model.ArchiveTitle,
            "Home",
            "عنوان بخش آرشیو");

        await SaveSetting(
            "Home.ArchiveDescription",
            model.ArchiveDescription,
            "Home",
            "توضیحات بخش آرشیو");

        await SaveSetting(
            "Home.ArchiveButtonText",
            model.ArchiveButtonText,
            "Home",
            "متن دکمه آرشیو");

        await SaveSetting(
            "Home.ArchiveButtonUrl",
            model.ArchiveButtonUrl,
            "Home",
            "لینک دکمه آرشیو");


        // -----------------------------------------------------
        // Speakers
        // -----------------------------------------------------

        await SaveSetting(
            "Home.SpeakersTitle",
            model.SpeakersTitle,
            "Home",
            "عنوان بخش سخنرانان");

        await SaveSetting(
            "Home.SpeakersDescription",
            model.SpeakersDescription,
            "Home",
            "توضیحات بخش سخنرانان");


        // -----------------------------------------------------
        // Footer
        // -----------------------------------------------------

        await SaveSetting(
            "Footer.Description",
            model.FooterDescription,
            "Footer",
            "توضیحات فوتر");

        await SaveSetting(
            "Footer.Phone",
            model.Phone,
            "Footer",
            "شماره تماس");

        await SaveSetting(
            "Footer.Email",
            model.Email,
            "Footer",
            "ایمیل");

        await SaveSetting(
            "Footer.Address",
            model.Address,
            "Footer",
            "آدرس");


        // -----------------------------------------------------
        // Social Media
        // -----------------------------------------------------

        await SaveSetting(
            "Footer.BaleUrl",
            model.BaleUrl,
            "Footer",
            "لینک بله");

        await SaveSetting(
            "Footer.EitaaUrl",
            model.EitaaUrl,
            "Footer",
            "لینک ایتا");

        await SaveSetting(
            "Footer.InstagramUrl",
            model.InstagramUrl,
            "Footer",
            "لینک اینستاگرام");

        await SaveSetting(
            "Footer.TelegramUrl",
            model.TelegramUrl,
            "Footer",
            "لینک تلگرام");


        await _context.SaveChangesAsync();


        TempData["Success"] =
            "محتوای سایت با موفقیت ذخیره شد.";


        return RedirectToAction(
            nameof(Home));
    }


    // =========================================================
    // GET SETTING VALUE
    // =========================================================

    private static string? GetValue(
        List<SiteSetting> settings,
        string key)
    {
        return settings
            .FirstOrDefault(x => x.Key == key)
            ?.Value;
    }


    // =========================================================
    // SAVE SETTING
    // =========================================================

    private async Task SaveSetting(
        string key,
        string? value,
        string group,
        string description)
    {
        var setting = await _context.SiteSettings
            .FirstOrDefaultAsync(x => x.Key == key);


        if (setting == null)
        {
            setting = new SiteSetting
            {
                Key = key,
                Value = value,
                Group = group,
                Description = description,
                IsPublic = true
            };

            _context.SiteSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            setting.Group = group;
            setting.Description = description;
        }
    }
}