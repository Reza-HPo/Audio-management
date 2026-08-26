using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.ViewModels.Home;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var audioQuery = _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished);


        var model = new HomeViewModel
        {
            AudioCount = await audioQuery.CountAsync(),

            SpeakerCount = await _context.Speakers
                .AsNoTracking()
                .CountAsync(s => s.IsActive),

            CategoryCount = await _context.Categories
                .AsNoTracking()
                .CountAsync(c => c.IsActive),


            LatestAudios = await audioQuery
                .Include(a => a.Speaker)
                .Include(a => a.AudioCategories)
                    .ThenInclude(ac => ac.Category)
                .OrderByDescending(a =>
                    a.PublishedAt ?? a.CreatedAt)
                .Take(6)
                .Select(a => new HomeAudioViewModel
                {
                    Id = a.Id,

                    Title = a.Title,

                    SpeakerName = a.Speaker != null
                        ? a.Speaker.Name
                        : null,

                    CoverImageUrl = a.CoverImageUrl,

                    FileName = a.FileName,

                    Duration = a.Duration,

                    PublishedAt = a.PublishedAt,

                    Categories = a.AudioCategories
                        .Where(ac => ac.Category != null)
                        .Select(ac => ac.Category.Name)
                        .ToList()
                })
                .ToListAsync(),


            Categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Select(c => new HomeCategoryViewModel
                {
                    Id = c.Id,

                    Name = c.Name,

                    AudioCount = c.AudioCategories
                        .Count(ac => ac.AudioFile.IsPublished)
                })
                .Take(8)
                .ToListAsync(),


            Speakers = await _context.Speakers
             .AsNoTracking()
             .Where(s => s.IsActive)
             .Select(s => new HomeSpeakerViewModel
             {
                 Id = s.Id,

                 Name = s.Name,

                 ImageUrl = s.ImageUrl,

                 AudioCount = _context.AudioFiles
                .Count(a =>
                  a.IsPublished &&
                  a.SpeakerId == s.Id)
              })
             .OrderByDescending(s => s.AudioCount)
             .ThenBy(s => s.Name)
             .Take(4)
             .ToListAsync()
        };


        return View(model);
    }
}