using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace Web.Controllers;

public class NoticesController : Controller
{
    private readonly ApplicationDbContext _context;

    public NoticesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Notices
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var notices = await _context.Notices
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
            .ToListAsync();

        return View(notices);
    }

    // GET: /Notices/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var notice = await _context.Notices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.IsPublished);

        if (notice == null)
            return NotFound();

        return View(notice);
    }
}