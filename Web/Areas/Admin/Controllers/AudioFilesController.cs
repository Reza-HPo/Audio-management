using FluentFTP;
using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Models.AudioFiles;
using Web.Services;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AudioFilesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly FtpAudioStorage _ftpStorage;

    public AudioFilesController(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        FtpAudioStorage ftpStorage)
    {
        _context = context;
        _environment = environment;
        _ftpStorage = ftpStorage;
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
    [RequestSizeLimit(500 * 1024 * 1024)]
    [RequestFormLimits(
        MultipartBodyLengthLimit = 500 * 1024 * 1024)]
    public async Task<IActionResult> Create(
        AudioFileCreateViewModel model)
    {
        // -----------------------------------------------------
        // بررسی فایل صوتی
        // -----------------------------------------------------

        if (model.Audio == null ||
            model.Audio.Length == 0)
        {
            ModelState.AddModelError(
                nameof(model.Audio),
                "لطفاً یک فایل صوتی انتخاب کنید.");
        }

        var allowedAudioExtensions = new[]
        {
            ".mp3",
            ".m4a"
        };

        string? audioExtension = null;

        if (model.Audio != null &&
            model.Audio.Length > 0)
        {
            audioExtension = Path
                .GetExtension(model.Audio.FileName)
                .ToLowerInvariant();

            if (!allowedAudioExtensions.Contains(
                    audioExtension))
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
        // بررسی کاور
        // -----------------------------------------------------

        var allowedImageExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        string? coverExtension = null;

        if (model.CoverImage != null &&
            model.CoverImage.Length > 0)
        {
            coverExtension = Path
                .GetExtension(model.CoverImage.FileName)
                .ToLowerInvariant();

            if (!allowedImageExtensions.Contains(
                    coverExtension))
            {
                ModelState.AddModelError(
                    nameof(model.CoverImage),
                    "فرمت تصویر کاور معتبر نیست.");
            }
        }

        // -----------------------------------------------------
        // اگر Validation خطا داشت
        // -----------------------------------------------------

        if (!ModelState.IsValid)
        {
            await LoadCreateData();

            return View(model);
        }

        // -----------------------------------------------------
        // متغیرهای فایل
        // -----------------------------------------------------

        string? uploadedAudioUrl = null;
        string? uploadedAudioFileName = null;

        string? coverImageUrl = null;
        string? coverPhysicalPath = null;

        try
        {
            // =================================================
            // 1. آپلود صوت روی FTP
            // =================================================

            uploadedAudioFileName =
                $"{Guid.NewGuid():N}{audioExtension}";

            await using (var audioStream =
                model.Audio!.OpenReadStream())
            {
                uploadedAudioUrl =
                    await _ftpStorage.UploadAsync(
                        audioStream,
                        uploadedAudioFileName);
            }

            // =================================================
            // 2. ذخیره کاور روی هاست اصلی
            // =================================================

            if (model.CoverImage != null &&
                model.CoverImage.Length > 0)
            {
                var coverFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "covers");

                Directory.CreateDirectory(coverFolder);

                var coverFileName =
                    $"{Guid.NewGuid():N}{coverExtension}";

                coverPhysicalPath = Path.Combine(
                    coverFolder,
                    coverFileName);

                await using (var coverStream =
                    new FileStream(
                        coverPhysicalPath,
                        FileMode.Create))
                {
                    await model.CoverImage
                        .CopyToAsync(coverStream);
                }

                coverImageUrl =
                    $"/uploads/covers/{coverFileName}";
            }

            // =================================================
            // 3. ساخت Entity
            // =================================================

            var now = DateTime.UtcNow;

            var audioFile = new AudioFile
            {
                Title = model.Title.Trim(),

                Description =
                    model.Description?.Trim(),

                // آدرس کامل فایل روی هاست دانلود
                FileName = uploadedAudioUrl!,

                CoverImageUrl = coverImageUrl,

                FileSize = model.Audio.Length,

                ContentType =
                    string.IsNullOrWhiteSpace(
                        model.Audio.ContentType)
                        ? GetAudioContentType(
                            audioExtension!)
                        : model.Audio.ContentType,

                SpeakerId = model.SpeakerId,

                IsPublished =
                    model.IsPublished,

                IsDownloadable =
                    model.IsDownloadable,

                CreatedAt = now,

                PublishedAt =
                    model.IsPublished
                        ? now
                        : null
            };

            // =================================================
            // 4. ذخیره صوت
            // =================================================

            _context.AudioFiles.Add(audioFile);

            await _context.SaveChangesAsync();

            // =================================================
            // 5. ذخیره دسته‌بندی‌ها
            // =================================================

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

                await _context.SaveChangesAsync();
            }

            TempData["Success"] =
                "فایل صوتی با موفقیت اضافه شد.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // -------------------------------------------------
            // اگر FTP آپلود شده ولی ادامه عملیات شکست خورد
            // فایل FTP را پاک می‌کنیم.
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    uploadedAudioFileName))
            {
                try
                {
                    await _ftpStorage.DeleteAsync(
                        uploadedAudioFileName);
                }
                catch
                {
                    // جلوگیری از مخفی شدن خطای اصلی
                }
            }

            // -------------------------------------------------
            // حذف کاور در صورت شکست
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    coverPhysicalPath) &&
                System.IO.File.Exists(
                    coverPhysicalPath))
            {
                try
                {
                    System.IO.File.Delete(
                        coverPhysicalPath);
                }
                catch
                {
                    // جلوگیری از مخفی شدن خطای اصلی
                }
            }

            ModelState.AddModelError(
                string.Empty,
                $"خطا در ذخیره فایل صوتی: {ex.Message}");

            await LoadCreateData();

            return View(model);
        }
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

            Description =
                audioFile.Description,

            SpeakerId =
                audioFile.SpeakerId,

            CategoryIds =
                audioFile.AudioCategories
                    .Select(ac => ac.CategoryId)
                    .ToList(),

            IsPublished =
                audioFile.IsPublished,

            IsDownloadable =
                audioFile.IsDownloadable,

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
    [RequestSizeLimit(500 * 1024 * 1024)]
    [RequestFormLimits(
        MultipartBodyLengthLimit = 500 * 1024 * 1024)]
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

        var allowedAudioExtensions = new[]
        {
            ".mp3",
            ".m4a"
        };

        string? newAudioExtension = null;

        if (model.Audio != null &&
            model.Audio.Length > 0)
        {
            newAudioExtension =
                Path.GetExtension(
                    model.Audio.FileName)
                    .ToLowerInvariant();

            if (!allowedAudioExtensions.Contains(
                    newAudioExtension))
            {
                ModelState.AddModelError(
                    nameof(model.Audio),
                    "فرمت فایل باید MP3 یا M4A باشد.");
            }
        }

        // -----------------------------------------------------
        // بررسی کاور جدید
        // -----------------------------------------------------

        var allowedImageExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        string? newCoverExtension = null;

        if (model.CoverImage != null &&
            model.CoverImage.Length > 0)
        {
            newCoverExtension =
                Path.GetExtension(
                    model.CoverImage.FileName)
                    .ToLowerInvariant();

            if (!allowedImageExtensions.Contains(
                    newCoverExtension))
            {
                ModelState.AddModelError(
                    nameof(model.CoverImage),
                    "فرمت تصویر کاور معتبر نیست.");
            }
        }

        // -----------------------------------------------------
        // اگر Validation خطا داشت
        // -----------------------------------------------------

        if (!ModelState.IsValid)
        {
            await LoadCreateData();

            return View(model);
        }

        string? newAudioFileName = null;
        string? newAudioUrl = null;

        string? newCoverUrl = null;
        string? newCoverPhysicalPath = null;

        // URL قدیمی
        var oldAudioUrl = audioFile.FileName;

        // نام فایل قدیمی FTP
        var oldAudioFileName =
            ExtractFtpFileName(oldAudioUrl);

        var oldCoverUrl =
            audioFile.CoverImageUrl;

        try
        {
            // =================================================
            // 1. اگر فایل صوتی جدید انتخاب شده
            // =================================================

            if (model.Audio != null &&
                model.Audio.Length > 0)
            {
                newAudioFileName =
                    $"{Guid.NewGuid():N}{newAudioExtension}";

                await using (var audioStream =
                    model.Audio.OpenReadStream())
                {
                    newAudioUrl =
                        await _ftpStorage.UploadAsync(
                            audioStream,
                            newAudioFileName);
                }
            }

            // =================================================
            // 2. اگر کاور جدید انتخاب شده
            // =================================================

            if (model.CoverImage != null &&
                model.CoverImage.Length > 0)
            {
                var coverFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "covers");

                Directory.CreateDirectory(
                    coverFolder);

                var newCoverFileName =
                    $"{Guid.NewGuid():N}{newCoverExtension}";

                newCoverPhysicalPath =
                    Path.Combine(
                        coverFolder,
                        newCoverFileName);

                await using (var coverStream =
                    new FileStream(
                        newCoverPhysicalPath,
                        FileMode.Create))
                {
                    await model.CoverImage
                        .CopyToAsync(coverStream);
                }

                newCoverUrl =
                    $"/uploads/covers/{newCoverFileName}";
            }

            // =================================================
            // 3. بروزرسانی اطلاعات
            // =================================================

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

            // -------------------------------------------------
            // PublishedAt
            // -------------------------------------------------

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

            // =================================================
            // 4. ثبت فایل صوتی جدید
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    newAudioUrl))
            {
                audioFile.FileName =
                    newAudioUrl;

                audioFile.FileSize =
                    model.Audio!.Length;

                audioFile.ContentType =
                    string.IsNullOrWhiteSpace(
                        model.Audio.ContentType)
                        ? GetAudioContentType(
                            newAudioExtension!)
                        : model.Audio.ContentType;
            }

            // =================================================
            // 5. ثبت کاور جدید
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    newCoverUrl))
            {
                audioFile.CoverImageUrl =
                    newCoverUrl;
            }

            // =================================================
            // 6. بروزرسانی دسته‌بندی‌ها
            // =================================================

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

            // =================================================
            // 7. ذخیره دیتابیس
            // =================================================

            await _context.SaveChangesAsync();

            // =================================================
            // 8. بعد از موفقیت DB:
            //    فایل صوتی قدیمی را از FTP حذف کن
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    newAudioUrl) &&
                !string.IsNullOrWhiteSpace(
                    oldAudioFileName))
            {
                try
                {
                    await _ftpStorage.DeleteAsync(
                        oldAudioFileName);
                }
                catch
                {
                    // عدم حذف فایل قدیمی نباید
                    // ویرایش موفق را خراب کند.
                }
            }

            // =================================================
            // 9. کاور قدیمی را حذف کن
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    newCoverUrl))
            {
                DeletePhysicalFile(
                    oldCoverUrl);
            }

            TempData["Success"] =
                "فایل صوتی با موفقیت ویرایش شد.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // -------------------------------------------------
            // فایل صوتی جدید را اگر آپلود شده بود حذف کن
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    newAudioFileName))
            {
                try
                {
                    await _ftpStorage.DeleteAsync(
                        newAudioFileName);
                }
                catch
                {
                    // جلوگیری از مخفی شدن خطای اصلی
                }
            }

            // -------------------------------------------------
            // کاور جدید را اگر ساخته شده بود حذف کن
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    newCoverPhysicalPath) &&
                System.IO.File.Exists(
                    newCoverPhysicalPath))
            {
                try
                {
                    System.IO.File.Delete(
                        newCoverPhysicalPath);
                }
                catch
                {
                    // جلوگیری از مخفی شدن خطای اصلی
                }
            }

            ModelState.AddModelError(
                string.Empty,
                $"خطا در ویرایش فایل صوتی: {ex.Message}");

            await LoadCreateData();

            return View(model);
        }
    }

    // =========================================================
    // DELETE
    // =========================================================

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

        var audioFileName =
            ExtractFtpFileName(audio.FileName);

        try
        {
            // =================================================
            // 1. حذف فایل صوتی از FTP
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    audioFileName))
            {
                try
                {
                    await _ftpStorage.DeleteAsync(
                        audioFileName);
                }
                catch (Exception ex)
                {
                    TempData["Error"] =
                        $"حذف فایل صوتی از هاست دانلود ناموفق بود: {ex.Message}";

                    return RedirectToAction(
                        nameof(Index));
                }
            }

            // =================================================
            // 2. حذف کاور از هاست اصلی
            // =================================================

            DeletePhysicalFile(
                audio.CoverImageUrl);

            // =================================================
            // 3. حذف دسته‌بندی‌ها
            // =================================================

            var categories =
                await _context.AudioCategories
                    .Where(x =>
                        x.AudioFileId == id)
                    .ToListAsync();

            if (categories.Any())
            {
                _context.AudioCategories
                    .RemoveRange(categories);
            }

            // =================================================
            // 4. حذف رکورد صوت
            // =================================================

            _context.AudioFiles.Remove(audio);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "فایل صوتی با موفقیت حذف شد.";

            return RedirectToAction(
                nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                $"خطا در حذف فایل صوتی: {ex.Message}";

            return RedirectToAction(
                nameof(Index));
        }
    }

    // =========================================================
    // LOAD CREATE DATA
    // =========================================================

    private async Task LoadCreateData()
    {
        ViewBag.Speakers =
            await _context.Speakers
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

        ViewBag.Categories =
            await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
    }

    // =========================================================
    // DELETE LOCAL PHYSICAL FILE
    // =========================================================

    private void DeletePhysicalFile(
        string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(
                relativePath))
        {
            return;
        }

        // اگر URL کامل باشد، فقط مسیر local
        // برای کاورها قابل استفاده است.
        if (Uri.TryCreate(
                relativePath,
                UriKind.Absolute,
                out var uri))
        {
            relativePath =
                uri.AbsolutePath;
        }

        var cleanPath =
            relativePath
                .TrimStart('/')
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);

        var physicalPath =
            Path.Combine(
                _environment.WebRootPath,
                cleanPath);

        if (System.IO.File.Exists(
                physicalPath))
        {
            System.IO.File.Delete(
                physicalPath);
        }
    }

    // =========================================================
    // EXTRACT FTP FILE NAME
    // =========================================================

    private string? ExtractFtpFileName(
        string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        try
        {
            // اگر آدرس کامل باشد
            if (Uri.TryCreate(
                    url,
                    UriKind.Absolute,
                    out var uri))
            {
                var fileName =
                    Path.GetFileName(
                        uri.AbsolutePath);

                return string.IsNullOrWhiteSpace(
                        fileName)
                    ? null
                    : fileName;
            }

            // اگر به هر دلیل فقط نام فایل ذخیره شده باشد
            return Path.GetFileName(url);
        }
        catch
        {
            return null;
        }
    }

    // =========================================================
    // AUDIO CONTENT TYPE
    // =========================================================

    private static string GetAudioContentType(
        string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".mp3" => "audio/mpeg",
            ".m4a" => "audio/mp4",
            _ => "application/octet-stream"
        };
    }
}