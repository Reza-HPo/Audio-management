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
    // دریافت لیست فایل‌های صوتی منتشرشده با Pagination
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAudios(
    [FromQuery] string? search = null,
    [FromQuery] int? speakerId = null,
    [FromQuery] int? categoryId = null,
    [FromQuery] string sort = "latest",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        // جلوگیری از مقادیر نامعتبر
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 20;

        // محدود کردن تعداد آیتم در هر صفحه
        if (pageSize > 50)
            pageSize = 50;

        // Query اصلی
        var query = _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished);

        // =========================================================
        // SEARCH
        // جستجو در عنوان و توضیحات صوت
        // =========================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(a =>
                a.Title.Contains(search) ||
                (a.Description != null &&
                 a.Description.Contains(search)));
        }

        // =========================================================
        // FILTER BY SPEAKER
        // =========================================================

        if (speakerId.HasValue)
        {
            query = query.Where(a =>
                a.SpeakerId == speakerId.Value);
        }


        // =========================================================
        // FILTER BY CATEGORY
        // =========================================================

        if (categoryId.HasValue)
        {
            query = query.Where(a =>
                a.AudioCategories.Any(ac =>
                    ac.CategoryId == categoryId.Value));
        }

        // تعداد کل رکوردها
        var totalCount = await query.CountAsync();

        // تعداد صفحات
        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);

        // دریافت اطلاعات صفحه موردنظر
        var audios = await query
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        return Ok(new
        {
            items = audios,
            page = page,
            pageSize = pageSize,
            totalCount = totalCount,
            totalPages = totalPages
        });
    }


    // =========================================================
    // GET: /api/audios/{id}
    // دریافت یک فایل صوتی بر اساس شناسه
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
        {
            return NotFound(new
            {
                message = "فایل صوتی مورد نظر پیدا نشد."
            });
        }

        return Ok(audio);
    }

    // =========================================================
    // GET: /api/audios/latest
    // دریافت آخرین فایل‌های صوتی منتشرشده
    // =========================================================

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAudios(
        [FromQuery] int count = 10)
    {
        // جلوگیری از درخواست‌های نامعتبر
        if (count < 1)
            count = 10;

        // محدود کردن تعداد نتایج
        if (count > 50)
            count = 50;

        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished)
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
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
            .ToListAsync();

        return Ok(audios);
    }

    // =========================================================
    // GET: /api/audios/speaker/{speakerId}
    // دریافت فایل‌های صوتی منتشرشده یک سخنران
    // =========================================================

    [HttpGet("speaker/{speakerId:int}")]
    public async Task<IActionResult> GetAudiosBySpeaker(int speakerId)
    {
        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a =>
                a.IsPublished &&
                a.SpeakerId == speakerId)
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .OrderByDescending(a => a.PublishedAt)
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
            .ToListAsync();

        return Ok(audios);
    }

    // =========================================================
    // GET: /api/audios/category/{categoryId}
    // دریافت فایل‌های صوتی منتشرشده یک دسته‌بندی
    // =========================================================

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetAudiosByCategory(int categoryId)
    {
        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a =>
                a.IsPublished &&
                a.AudioCategories.Any(ac =>
                    ac.CategoryId == categoryId))
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .OrderByDescending(a => a.PublishedAt)
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
            .ToListAsync();

        return Ok(audios);
    }
}