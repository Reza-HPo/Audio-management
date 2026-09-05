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
    // دریافت یک سخنران بر اساس شناسه
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSpeaker(int id)
    {
        var speaker = await _context.Speakers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                ImageUrl = s.ImageUrl
            })
            .FirstOrDefaultAsync();

        if (speaker == null)
        {
            return NotFound(new
            {
                message = "سخنران مورد نظر پیدا نشد."
            });
        }

        return Ok(speaker);
    }
}