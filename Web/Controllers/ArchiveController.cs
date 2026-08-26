using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.ViewModels.Archive;

namespace Web.Controllers;

public class ArchiveController : Controller
{
    private readonly ApplicationDbContext _context;

    private const int PageSize = 12;

    public ArchiveController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, int? speakerId, string sort = "newest", int page = 1)
    {
        if (page < 1)
            page = 1;

        var query = _context.AudioFiles
            .AsNoTracking()
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .Where(a => a.IsPublished)
            .AsQueryable();


        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(a =>
                a.Title.Contains(search) ||
                (a.Description != null &&
                 a.Description.Contains(search)));
        }


        // Category Filter
        if (categoryId.HasValue)
        {
            query = query.Where(a =>
                a.AudioCategories.Any(ac =>
                    ac.CategoryId == categoryId.Value));
        }


        // Speaker Filter
        if (speakerId.HasValue)
        {
            query = query.Where(a =>
                a.SpeakerId == speakerId.Value);
        }


        // Sort
        query = sort.ToLower() switch
        {
            "oldest" =>
                query.OrderBy(a =>
                    a.PublishedAt ?? a.CreatedAt),

            "alphabetical" =>
                query.OrderBy(a => a.Title),

            _ =>
                query.OrderByDescending(a =>
                    a.PublishedAt ?? a.CreatedAt)
        };


        // Total count
        var totalCount = await query.CountAsync();


        // Pagination
        var totalPages =
            (int)Math.Ceiling(
                totalCount / (double)PageSize);

        if (totalPages > 0 && page > totalPages)
            page = totalPages;


        var audios = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();


        // ViewModel
        var model = new ArchiveViewModel
        {
            Search = search,

            CategoryId = categoryId,

            SpeakerId = speakerId,

            Sort = sort,

            TotalCount = totalCount,

            CurrentPage = page,

            TotalPages = totalPages,

            Audios = audios.Select(a => new AudioItemViewModel
            {
                Id = a.Id,

                Title = a.Title,

                Description = a.Description,

                SpeakerName = a.Speaker?.Name,

                CoverImageUrl = a.CoverImageUrl,

                FileName = a.FileName,

                Duration = a.Duration,

                PublishedAt = a.PublishedAt,

                Categories = a.AudioCategories
        .Select(ac => ac.Category.Name)
        .ToList()

            }).ToList(),

            Categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryFilterViewModel
                {
                    Id = c.Id,

                    Name = c.Name
                })
                .ToListAsync(),

            Speakers = await _context.Speakers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new SpeakerFilterViewModel
                {
                    Id = s.Id,

                    Name = s.Name
                })
                .ToListAsync()
        };


        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var audio = await _context.AudioFiles
            .AsNoTracking()
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.IsPublished);

        if (audio == null)
            return NotFound();

        var model = new AudioItemViewModel
        {
            Id = audio.Id,

            Title = audio.Title,

            Description = audio.Description,

            SpeakerName = audio.Speaker?.Name,

            CoverImageUrl = audio.CoverImageUrl,

            FileName = audio.FileName,

            Duration = audio.Duration,

            PublishedAt = audio.PublishedAt,

            IsDownloadable = audio.IsDownloadable,

            Categories = audio.AudioCategories
                .Where(ac => ac.Category != null)
                .Select(ac => ac.Category.Name)
                .ToList()
        };

        return View(model);
    }
}