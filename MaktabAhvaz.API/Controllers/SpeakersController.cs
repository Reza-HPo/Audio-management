using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeakersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SpeakersController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: /api/speakers
    // دریافت لیست سخنران‌ها
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetSpeakers()
    {
        var speakers = await _context.Speakers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                ImageUrl = s.ImageUrl
            })
            .ToListAsync();

        return Ok(speakers);
    }

    // =========================================================
    // GET: /api/speakers/{id}
    // دریافت جزئیات سخنران به همراه فایل‌های صوتی
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSpeaker(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var speaker = await _context.Speakers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                Id = s.Id,
                Name = s.Name,

                ImageUrl = string.IsNullOrWhiteSpace(s.ImageUrl)
                    ? null
                    : $"{baseUrl}{s.ImageUrl}"
            })
            .FirstOrDefaultAsync();

        if (speaker == null)
        {
            return NotFound(new
            {
                message = "سخنران مورد نظر پیدا نشد."
            });
        }

        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a =>
                a.SpeakerId == id &&
                a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
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
                PublishedAt = a.PublishedAt
            })
            .ToListAsync();

        return Ok(new
        {
            speaker.Id,
            speaker.Name,
            speaker.ImageUrl,
            audioCount = audios.Count,
            audios
        });
    }
}