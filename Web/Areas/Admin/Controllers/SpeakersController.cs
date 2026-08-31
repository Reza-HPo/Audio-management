using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SpeakersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public SpeakersController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: /Admin/Speakers
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var speakers = await _context.Speakers.AsNoTracking().OrderBy(s => s.Name).ToListAsync();

        return View(speakers);
    }

    // GET: /Admin/Speakers/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Admin/Speakers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string name,
        string? bio,
        IFormFile? image,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "Name",
                "نام سخنران الزامی است.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        string? imageUrl = null;

        if (image != null && image.Length > 0)
        {
            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path
                .GetExtension(image.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    "Image",
                    "فرمت تصویر باید JPG، JPEG، PNG یا WEBP باشد.");

                return View();
            }

            var imageFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "speakers");

            Directory.CreateDirectory(imageFolder);

            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            var filePath = Path.Combine(
                imageFolder,
                fileName);

            await using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await image.CopyToAsync(stream);

            imageUrl =
                $"/uploads/speakers/{fileName}";
        }

        var speaker = new Speaker
        {
            Name = name.Trim(),
            Bio = bio?.Trim(),
            ImageUrl = imageUrl,
            IsActive = isActive
        };

        _context.Speakers.Add(speaker);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "سخنران با موفقیت اضافه شد.";

        return RedirectToAction(nameof(Index));
    }

// =========================================================
// DELETE - GET
// =========================================================

// GET: /Admin/Speakers/Delete/5
[HttpGet]
public async Task<IActionResult> Delete(int id)
    {
        var speaker = await _context.Speakers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (speaker == null)
        {
            return NotFound();
        }

        return View(speaker);
    }


    // =========================================================
    // DELETE - POST
    // =========================================================

    // POST: /Admin/Speakers/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var speaker = await _context.Speakers
            .FirstOrDefaultAsync(s => s.Id == id);

        if (speaker == null)
        {
            return NotFound();
        }


        // ---------------------------------------------------------
        // بررسی فایل‌های صوتی وابسته
        // ---------------------------------------------------------

        var hasAudioFiles = await _context.AudioFiles
            .AnyAsync(a => a.SpeakerId == id);

        if (hasAudioFiles)
        {
            TempData["Error"] =
                "این سخنران دارای فایل صوتی است و نمی‌توان آن را حذف کرد.";

            return RedirectToAction(nameof(Index));
        }


        // ---------------------------------------------------------
        // حذف تصویر سخنران
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(speaker.ImageUrl))
        {
            var imagePath = Path.Combine(
                _environment.WebRootPath,
                speaker.ImageUrl
                    .TrimStart('/')
                    .Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
            );

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }


        // ---------------------------------------------------------
        // حذف رکورد
        // ---------------------------------------------------------

        _context.Speakers.Remove(speaker);

        await _context.SaveChangesAsync();


        // ---------------------------------------------------------
        // پیام موفقیت
        // ---------------------------------------------------------

        TempData["Success"] =
            "سخنران با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }

}