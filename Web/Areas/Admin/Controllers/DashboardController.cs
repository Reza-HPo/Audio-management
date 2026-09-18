using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            AudioFilesCount = await _context.AudioFiles.CountAsync(),
            SpeakersCount = await _context.Speakers.CountAsync(),
            CategoriesCount = await _context.Categories.CountAsync()
        };

        return View(model);
    }
}

public class DashboardViewModel
{
    public int AudioFilesCount { get; set; }

    public int SpeakersCount { get; set; }

    public int CategoriesCount { get; set; }

    public int PagesCount { get; set; }
}