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
    // CREATE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "اطلاعات وارد شده صحیح نیست."
            });
        }


        // بررسی نام تکراری

        var nameExists = await _context.Categories
            .AnyAsync(c => c.Name == model.Name);

        if (nameExists)
        {
            return BadRequest(new
            {
                success = false,
                message = "دسته‌بندی با این نام قبلاً وجود دارد."
            });
        }


        // Slug

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = GenerateSlug(model.Name);
        }


        model.Id = 0;

        _context.Categories.Add(model);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            success = true,
            message = "دسته‌بندی با موفقیت ایجاد شد.",
            id = model.Id,
            name = model.Name,
            slug = model.Slug,
            description = model.Description,
            isActive = model.IsActive,
            displayOrder = model.DisplayOrder
        });
    }


    // =========================================================
    // EDIT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.Id)
        {
            return BadRequest(new
            {
                success = false,
                message = "شناسه دسته‌بندی نامعتبر است."
            });
        }


        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new
            {
                success = false,
                message = "دسته‌بندی پیدا نشد."
            });
        }


        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "اطلاعات وارد شده صحیح نیست."
            });
        }


        // بررسی نام تکراری

        var duplicateName = await _context.Categories
            .AnyAsync(c =>
                c.Id != id &&
                c.Name == model.Name);

        if (duplicateName)
        {
            return BadRequest(new
            {
                success = false,
                message = "دسته‌بندی دیگری با این نام وجود دارد."
            });
        }


        category.Name = model.Name;
        category.Description = model.Description;
        category.Slug = string.IsNullOrWhiteSpace(model.Slug)
            ? GenerateSlug(model.Name)
            : model.Slug;

        category.IsActive = model.IsActive;
        category.DisplayOrder = model.DisplayOrder;


        await _context.SaveChangesAsync();


        return Ok(new
        {
            success = true,
            message = "دسته‌بندی با موفقیت ویرایش شد.",
            id = category.Id,
            name = category.Name,
            slug = category.Slug,
            description = category.Description,
            isActive = category.IsActive,
            displayOrder = category.DisplayOrder
        });
    }


    // =========================================================
    // DELETE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.AudioCategories)
            .FirstOrDefaultAsync(c => c.Id == id);


        if (category == null)
        {
            return NotFound(new
            {
                success = false,
                message = "دسته‌بندی پیدا نشد."
            });
        }


        // دسته‌بندی دارای فایل صوتی است

        if (category.AudioCategories.Any())
        {
            return BadRequest(new
            {
                success = false,
                message =
                    "این دسته‌بندی به فایل‌های صوتی متصل است و قابل حذف نیست."
            });
        }


        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            success = true,
            message = "دسته‌بندی با موفقیت حذف شد.",
            id = id
        });
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
