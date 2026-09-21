using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaktabAhvaz.Infrastructure.Data;

namespace Web.Controllers;

public class SpeakersController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpeakersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Speakers
    public async Task<IActionResult> Index()
    {
        var speakers = await _context.Speakers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(speakers);
    }

    // GET: /Speakers/Details/5
    public async Task<IActionResult> Details(int id)
    {
        // دریافت سخنران
        var speaker = await _context.Speakers
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == id &&
                s.IsActive);

        if (speaker == null)
        {
            return NotFound();
        }

        // دریافت سخنرانی‌های منتشرشده این سخنران
        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a =>
                a.SpeakerId == id &&
                a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();

        // ارسال اطلاعات به View
        ViewBag.Audios = audios;

        return View(speaker);
    }
}