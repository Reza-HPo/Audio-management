using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: /api/categories
    // دریافت لیست دسته‌بندی‌ها
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(categories);
    }

    // =========================================================
    // GET: /api/categories/{id}
    // دریافت جزئیات دسته‌بندی به همراه فایل‌های صوتی
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound(new
            {
                message = "دسته‌بندی مورد نظر پیدا نشد."
            });
        }

        var audios = await _context.AudioCategories
            .AsNoTracking()
            .Where(ac =>
                ac.CategoryId == id &&
                ac.AudioFile != null &&
                ac.AudioFile.IsPublished)
            .OrderByDescending(ac => ac.AudioFile!.PublishedAt)
            .Select(ac => new
            {
                Id = ac.AudioFile!.Id,
                Title = ac.AudioFile.Title,
                Description = ac.AudioFile.Description,

                FileUrl = string.IsNullOrWhiteSpace(ac.AudioFile.FileName)
                    ? null
                    : $"{baseUrl}{ac.AudioFile.FileName}",

                CoverImageUrl = string.IsNullOrWhiteSpace(ac.AudioFile.CoverImageUrl)
                    ? null
                    : $"{baseUrl}{ac.AudioFile.CoverImageUrl}",

                Duration = ac.AudioFile.Duration,
                PublishedAt = ac.AudioFile.PublishedAt
            })
            .ToListAsync();

        return Ok(new
        {
            category.Id,
            category.Name,
            audioCount = audios.Count,
            audios
        });
    }
}