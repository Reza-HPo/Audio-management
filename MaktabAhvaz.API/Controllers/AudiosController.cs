using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudiosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AudiosController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: /api/audios
    // دریافت لیست فایل‌های صوتی منتشرشده
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAudio(int id)
    {
        var audio = await _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.Id == id && a.IsPublished)
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .Select(a => new
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                FileName = a.FileName,
                CoverImageUrl = a.CoverImageUrl,
                Duration = a.Duration,
                PublishedAt = a.PublishedAt,

                Speaker = a.Speaker == null
                    ? null
                    : new
                    {
                        Id = a.Speaker.Id,
                        Name = a.Speaker.Name,
                        ImageUrl = a.Speaker.ImageUrl
                    },

                Categories = a.AudioCategories
                    .Where(ac => ac.Category != null)
                    .Select(ac => new
                    {
                        Id = ac.Category.Id,
                        Name = ac.Category.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (audio == null)
            return NotFound(new
            {
                message = "فایل صوتی مورد نظر پیدا نشد."
            });

        return Ok(audio);
    }
}
