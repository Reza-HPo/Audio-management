using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.AudioCategories)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // بررسی نام تکراری

        var nameExists = await _context.Categories
            .AnyAsync(c => c.Name == model.Name);

        if (nameExists)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "دسته‌بندی با این نام قبلاً وجود دارد."
            );

            return View(model);
        }


        // ساخت Slug در صورت خالی بودن

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = GenerateSlug(model.Name);
        }


        model.Id = 0;

        _context.Categories.Add(model);

        await _context.SaveChangesAsync();


        TempData["Success"] =
            "دسته‌بندی با موفقیت ایجاد شد.";


        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }


    // =========================================================
    // EDIT - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }


        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // بررسی نام تکراری

        var duplicateName = await _context.Categories
            .AnyAsync(c =>
                c.Id != id &&
                c.Name == model.Name);

        if (duplicateName)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "دسته‌بندی دیگری با این نام وجود دارد."
            );

            return View(model);
        }


        // بروزرسانی اطلاعات

        category.Name = model.Name;

        category.Description = model.Description;

        category.Slug =
            string.IsNullOrWhiteSpace(model.Slug)
                ? GenerateSlug(model.Name)
                : model.Slug;

        category.IsActive = model.IsActive;

        category.DisplayOrder = model.DisplayOrder;


        await _context.SaveChangesAsync();


        TempData["Success"] =
            "دسته‌بندی با موفقیت ویرایش شد.";


        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // DELETE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.AudioCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }


    // =========================================================
    // DELETE - POST
    // =========================================================

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.AudioCategories)
            .FirstOrDefaultAsync(c => c.Id == id);


        if (category == null)
        {
            return NotFound();
        }


        // اگر دسته‌بندی به فایل صوتی متصل باشد،
        // اجازه حذف آن را نمی‌دهیم.

        if (category.AudioCategories.Any())
        {
            TempData["Error"] =
                "این دسته‌بندی به فایل‌های صوتی متصل است و قابل حذف نیست.";

            return RedirectToAction(nameof(Index));
        }


        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();


        TempData["Success"] =
            "دسته‌بندی با موفقیت حذف شد.";


        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // SLUG GENERATOR
    // =========================================================

    private static string GenerateSlug(string text)
    {
        return text
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("‌", "-");
    }
}