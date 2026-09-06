using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET: /api/home
    // اطلاعات مورد نیاز صفحه اصلی
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetHome()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        // =========================================================
        // آخرین فایل‌های صوتی
        // =========================================================

        var latestAudios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(10)
            .Select(a => new
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,

                FileUrl = string.IsNullOrWhiteSpace(a.FileName)
                    ? null
                    : $"{baseUrl}{a.FileName}",

                CoverImageUrl = string.IsNullOrWhiteSpace(a.CoverImageUrl)
                    ? null
                    : $"{baseUrl}{a.CoverImageUrl}",

                Duration = a.Duration,
                PublishedAt = a.PublishedAt,

                Speaker = a.Speaker == null
                    ? null
                    : new
                    {
                        Id = a.Speaker.Id,
                        Name = a.Speaker.Name,
                        ImageUrl = string.IsNullOrWhiteSpace(a.Speaker.ImageUrl)
                            ? null
                            : $"{baseUrl}{a.Speaker.ImageUrl}"
                    }
            })
            .ToListAsync();

        // =========================================================
        // سخنران‌ها
        // =========================================================

        var speakers = await _context.Speakers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                Id = s.Id,
                Name = s.Name,

                ImageUrl = string.IsNullOrWhiteSpace(s.ImageUrl)
                    ? null
                    : $"{baseUrl}{s.ImageUrl}"
            })
            .ToListAsync();

        // =========================================================
        // دسته‌بندی‌ها
        // =========================================================

        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        // =========================================================
        // خروجی نهایی
        // =========================================================

        return Ok(new
        {
            latestAudios,
            speakers,
            categories
        });
    }
}