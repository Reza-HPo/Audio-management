using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaktabAhvaz.Infrastructure.Data;
using Web.Models.Categories;

namespace Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET: /Categories
    // نمایش لیست دسته‌بندی‌ها
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Slug = c.Slug,

                AudioCount = c.AudioCategories
                    .Count(ac => ac.AudioFile.IsPublished)
            })
            .ToListAsync();

        return View(categories);
    }


    // =========================================================
    // GET: /Categories/Details/5
    // نمایش جزئیات یک دسته‌بندی
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c =>
                c.Id == id &&
                c.IsActive)
            .Select(c => new CategoryDetailsViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Slug = c.Slug,

                Audios = c.AudioCategories
                    .Where(ac =>
                        ac.AudioFile.IsPublished)
                    .OrderByDescending(ac =>
                        ac.AudioFile.PublishedAt)
                    .Select(ac => new CategoryAudioViewModel
                    {
                        Id = ac.AudioFile.Id,

                        Title = ac.AudioFile.Title,

                        Description = ac.AudioFile.Description,

                        FileName = ac.AudioFile.FileName,

                        CoverImageUrl = ac.AudioFile.CoverImageUrl,

                        Duration = ac.AudioFile.Duration,

                        FileSize = ac.AudioFile.FileSize,

                        PublishedAt = ac.AudioFile.PublishedAt,

                        SpeakerId = ac.AudioFile.SpeakerId,

                        SpeakerName = ac.AudioFile.Speaker.Name,

                        SpeakerImageUrl = ac.AudioFile.Speaker.ImageUrl
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }
}