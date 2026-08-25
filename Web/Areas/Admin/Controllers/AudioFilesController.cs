using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Models.AudioFiles;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AudioFilesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AudioFilesController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var audioFiles = await _context.AudioFiles
            .AsNoTracking()
            .Include(a => a.Speaker)
            .Include(a => a.AudioCategories)
                .ThenInclude(ac => ac.Category)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return View(audioFiles);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCreateData();

        return View();
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AudioFileCreateViewModel model)
    {
        // -----------------------------------------------------
        // بررسی فایل صوتی
        // -----------------------------------------------------

        if (model.Audio == null || model.Audio.Length == 0)
        {
            ModelState.AddModelError(
                nameof(model.Audio),
                "لطفاً یک فایل صوتی انتخاب کنید.");
        }


        // -----------------------------------------------------
        // بررسی فرمت فایل صوتی
        // -----------------------------------------------------

        var allowedAudioExtensions = new[]
        {
            ".mp3",
            ".m4a"
        };

        if (model.Audio != null)
        {
            var extension = Path
                .GetExtension(model.Audio.FileName)
                .ToLowerInvariant();

            if (!allowedAudioExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    nameof(model.Audio),
                    "فرمت فایل باید MP3 یا M4A باشد.");
            }
        }


        // -----------------------------------------------------
        // بررسی سخنران
        // -----------------------------------------------------

        var speakerExists = await _context.Speakers
            .AnyAsync(s =>
                s.Id == model.SpeakerId &&
                s.IsActive);

        if (!speakerExists)
        {
            ModelState.AddModelError(
                nameof(model.SpeakerId),
                "سخنران انتخاب‌شده معتبر نیست.");
        }


        // -----------------------------------------------------
        // Validation
        // -----------------------------------------------------

        if (!ModelState.IsValid)
        {
            await LoadCreateData();

            return View(model);
        }


        // -----------------------------------------------------
        // پوشه فایل صوتی
        // -----------------------------------------------------

        var audioFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "audio");

        Directory.CreateDirectory(audioFolder);


        // -----------------------------------------------------
        // نام یکتا
        // -----------------------------------------------------

        var audioExtension = Path
            .GetExtension(model.Audio!.FileName)
            .ToLowerInvariant();

        var audioFileName =
            $"{Guid.NewGuid():N}{audioExtension}";

        var audioPath = Path.Combine(
            audioFolder,
            audioFileName);


        // -----------------------------------------------------
        // ذخیره فایل صوتی
        // -----------------------------------------------------

        await using (var stream = new FileStream(
            audioPath,
            FileMode.Create))
        {
            await model.Audio.CopyToAsync(stream);
        }


        // -----------------------------------------------------
        // تصویر کاور
        // -----------------------------------------------------

        string? coverImageUrl = null;

        if (model.CoverImage != null &&
            model.CoverImage.Length > 0)
        {
            var allowedImageExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var coverExtension = Path
                .GetExtension(model.CoverImage.FileName)
                .ToLowerInvariant();

            if (!allowedImageExtensions.Contains(
                    coverExtension))
            {
                ModelState.AddModelError(
                    nameof(model.CoverImage),
                    "فرمت تصویر کاور معتبر نیست.");

                System.IO.File.Delete(audioPath);

                await LoadCreateData();

                return View(model);
            }


            var coverFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "covers");

            Directory.CreateDirectory(coverFolder);


            var coverFileName =
                $"{Guid.NewGuid():N}{coverExtension}";

            var coverPath = Path.Combine(
                coverFolder,
                coverFileName);


            await using (var stream = new FileStream(
                coverPath,
                FileMode.Create))
            {
                await model.CoverImage.CopyToAsync(stream);
            }


            coverImageUrl =
                $"/uploads/covers/{coverFileName}";
        }


        // -----------------------------------------------------
        // ساخت Entity
        // -----------------------------------------------------

        var audioFile = new AudioFile
        {
            Title = model.Title.Trim(),

            Description =
                model.Description?.Trim(),

            FileName =
                $"/uploads/audio/{audioFileName}",

            CoverImageUrl =
                coverImageUrl,

            FileSize =
                model.Audio.Length,

            ContentType =
                model.Audio.ContentType,

            SpeakerId =
                model.SpeakerId,

            IsPublished =
                model.IsPublished,

            IsDownloadable =
                model.IsDownloadable,

            CreatedAt =
                DateTime.UtcNow,

            PublishedAt =
                model.IsPublished
                    ? DateTime.UtcNow
                    : null
        };


        _context.AudioFiles.Add(audioFile);

        await _context.SaveChangesAsync();


        // -----------------------------------------------------
        // دسته‌بندی‌ها
        // -----------------------------------------------------

        if (model.CategoryIds != null &&
            model.CategoryIds.Count > 0)
        {
            var validCategoryIds =
                await _context.Categories
                    .Where(c =>
                        c.IsActive &&
                        model.CategoryIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();


            foreach (var categoryId in validCategoryIds)
            {
                _context.AudioCategories.Add(
                    new AudioCategory
                    {
                        AudioFileId =
                            audioFile.Id,

                        CategoryId =
                            categoryId
                    });
            }


            await _context.SaveChangesAsync();
        }


        TempData["Success"] =
            "فایل صوتی با موفقیت اضافه شد.";


        return RedirectToAction(
            nameof(Index));
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var audioFile = await _context.AudioFiles
            .Include(a => a.AudioCategories)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (audioFile == null)
        {
            return NotFound();
        }


        await LoadCreateData();


        var model = new AudioFileEditViewModel
        {
            Id = audioFile.Id,

            Title = audioFile.Title,

            Description = audioFile.Description,

            SpeakerId = audioFile.SpeakerId,

            CategoryIds = audioFile.AudioCategories
                .Select(ac => ac.CategoryId)
                .ToList(),

            IsPublished = audioFile.IsPublished,

            IsDownloadable = audioFile.IsDownloadable,

            CurrentAudioFile =
                audioFile.FileName,

            CurrentCoverImage =
                audioFile.CoverImageUrl
        };


        return View(model);
    }

    // =========================================================
    // EDIT - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        AudioFileEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }


        var audioFile = await _context.AudioFiles
            .Include(a => a.AudioCategories)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (audioFile == null)
        {
            return NotFound();
        }


        // -----------------------------------------------------
        // بررسی سخنران
        // -----------------------------------------------------

        var speakerExists = await _context.Speakers
            .AnyAsync(s =>
                s.Id == model.SpeakerId &&
                s.IsActive);

        if (!speakerExists)
        {
            ModelState.AddModelError(
                nameof(model.SpeakerId),
                "سخنران انتخاب‌شده معتبر نیست.");
        }


        // -----------------------------------------------------
        // بررسی فایل صوتی جدید
        // -----------------------------------------------------

        if (model.Audio != null &&
            model.Audio.Length > 0)
        {
            var allowedAudioExtensions = new[]
            {
                ".mp3",
                ".m4a"
            };

            var extension = Path
                .GetExtension(model.Audio.FileName)
                .ToLowerInvariant();

            if (!allowedAudioExtensions.Contains(
                    extension))
            {
                ModelState.AddModelError(
                    nameof(model.Audio),
                    "فرمت فایل باید MP3 یا M4A باشد.");
            }
        }


        // -----------------------------------------------------
        // بررسی کاور جدید
        // -----------------------------------------------------

        if (model.CoverImage != null &&
            model.CoverImage.Length > 0)
        {
            var allowedImageExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path
                .GetExtension(model.CoverImage.FileName)
                .ToLowerInvariant();

            if (!allowedImageExtensions.Contains(
                    extension))
            {
                ModelState.AddModelError(
                    nameof(model.CoverImage),
                    "فرمت تصویر کاور معتبر نیست.");
            }
        }


        // -----------------------------------------------------
        // Validation
        // -----------------------------------------------------

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value != null && x.Value.Errors.Any())
                .SelectMany(x => x.Value!.Errors.Select(e =>
                    $"{x.Key}: {e.ErrorMessage}"));

            return Content(
                string.Join(Environment.NewLine, errors));
        }


        // -----------------------------------------------------
        // اطلاعات اصلی
        // -----------------------------------------------------

        audioFile.Title =
            model.Title.Trim();

        audioFile.Description =
            model.Description?.Trim();

        audioFile.SpeakerId =
            model.SpeakerId;

        audioFile.IsPublished =
            model.IsPublished;

        audioFile.IsDownloadable =
            model.IsDownloadable;


        // -----------------------------------------------------
        // تاریخ انتشار
        // -----------------------------------------------------

        if (model.IsPublished)
        {
            if (!audioFile.PublishedAt.HasValue)
            {
                audioFile.PublishedAt =
                    DateTime.UtcNow;
            }
        }
        else
        {
            audioFile.PublishedAt = null;
        }


        // -----------------------------------------------------
        // فایل صوتی جدید
        // -----------------------------------------------------

        if (model.Audio != null &&
            model.Audio.Length > 0)
        {
            var audioFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "audio");

            Directory.CreateDirectory(audioFolder);


            var extension = Path
                .GetExtension(model.Audio.FileName)
                .ToLowerInvariant();


            var newFileName =
                $"{Guid.NewGuid():N}{extension}";


            var newFilePath = Path.Combine(
                audioFolder,
                newFileName);


            await using (var stream = new FileStream(
                newFilePath,
                FileMode.Create))
            {
                await model.Audio.CopyToAsync(stream);
            }


            // حذف فایل صوتی قبلی
            DeletePhysicalFile(
                audioFile.FileName);


            audioFile.FileName =
                $"/uploads/audio/{newFileName}";

            audioFile.FileSize =
                model.Audio.Length;

            audioFile.ContentType =
                model.Audio.ContentType;
        }


        // -----------------------------------------------------
        // کاور جدید
        // -----------------------------------------------------

        if (model.CoverImage != null &&
            model.CoverImage.Length > 0)
        {
            var coverFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "covers");

            Directory.CreateDirectory(
                coverFolder);


            var extension = Path
                .GetExtension(model.CoverImage.FileName)
                .ToLowerInvariant();


            var newCoverFileName =
                $"{Guid.NewGuid():N}{extension}";


            var newCoverPath = Path.Combine(
                coverFolder,
                newCoverFileName);


            await using (var stream = new FileStream(
                newCoverPath,
                FileMode.Create))
            {
                await model.CoverImage.CopyToAsync(
                    stream);
            }


            // حذف کاور قبلی
            DeletePhysicalFile(
                audioFile.CoverImageUrl);


            audioFile.CoverImageUrl =
                $"/uploads/covers/{newCoverFileName}";
        }


        // -----------------------------------------------------
        // دسته‌بندی‌ها
        // -----------------------------------------------------

        _context.AudioCategories.RemoveRange(
            audioFile.AudioCategories);


        if (model.CategoryIds != null &&
            model.CategoryIds.Count > 0)
        {
            var validCategoryIds =
                await _context.Categories
                    .Where(c =>
                        c.IsActive &&
                        model.CategoryIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();


            foreach (var categoryId
                     in validCategoryIds)
            {
                _context.AudioCategories.Add(
                    new AudioCategory
                    {
                        AudioFileId =
                            audioFile.Id,

                        CategoryId =
                            categoryId
                    });
            }
        }


        // -----------------------------------------------------
        // ذخیره
        // -----------------------------------------------------

        await _context.SaveChangesAsync();


        TempData["Success"] =
            "فایل صوتی با موفقیت ویرایش شد.";


        return RedirectToAction(
            nameof(Index));
    }


    // =========================================================
    // LOAD CREATE DATA
    // =========================================================

    private async Task LoadCreateData()
    {
        ViewBag.Speakers = await _context.Speakers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();


        ViewBag.Categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }


    // =========================================================
    // DELETE PHYSICAL FILE
    // =========================================================

    private void DeletePhysicalFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }


        var cleanPath = relativePath
            .TrimStart('/')
            .Replace(
                '/',
                Path.DirectorySeparatorChar);


        var physicalPath = Path.Combine(
            _environment.WebRootPath,
            cleanPath);


        if (System.IO.File.Exists(
                physicalPath))
        {
            System.IO.File.Delete(
                physicalPath);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var audio = await _context.AudioFiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (audio == null)
        {
            return NotFound();
        }

        // حذف فایل صوتی از دیسک
        if (!string.IsNullOrWhiteSpace(audio.FileName))
        {
            var audioPath = Path.Combine(
                _environment.WebRootPath,
                audio.FileName.TrimStart('/')
                             .Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(audioPath))
            {
                System.IO.File.Delete(audioPath);
            }
        }

        // حذف کاور
        if (!string.IsNullOrWhiteSpace(audio.CoverImageUrl))
        {
            var coverPath = Path.Combine(
                _environment.WebRootPath,
                audio.CoverImageUrl.TrimStart('/')
                                  .Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(coverPath))
            {
                System.IO.File.Delete(coverPath);
            }
        }

        // حذف ارتباط‌های دسته‌بندی
        var categories = await _context.AudioCategories
            .Where(x => x.AudioFileId == id)
            .ToListAsync();

        if (categories.Any())
        {
            _context.AudioCategories.RemoveRange(categories);
        }

        // حذف رکورد اصلی
        _context.AudioFiles.Remove(audio);

        await _context.SaveChangesAsync();

        TempData["Success"] = "فایل صوتی با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}