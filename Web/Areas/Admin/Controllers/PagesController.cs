using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public PagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Pages
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pages = await _context.Pages
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(pages);
    }

    // GET: Admin/Pages/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Pages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Page model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var slugExists = await _context.Pages
            .AnyAsync(x => x.Slug == model.Slug);

        if (slugExists)
        {
            ModelState.AddModelError(
                nameof(model.Slug),
                "صفحه‌ای با این آدرس قبلاً وجود دارد."
            );

            return View(model);
        }

        model.Id = 0;
        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = null;

        if (model.IsPublished && model.PublishedAt == null)
            model.PublishedAt = DateTime.UtcNow;

        _context.Pages.Add(model);

        await _context.SaveChangesAsync();

        TempData["Success"] = "صفحه با موفقیت ایجاد شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Pages/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var page = await _context.Pages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (page == null)
            return NotFound();

        return View(page);
    }

    // POST: Admin/Pages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Page model)
    {
        if (id != model.Id)
            return BadRequest();

        var page = await _context.Pages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (page == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var slugExists = await _context.Pages
            .AnyAsync(x => x.Id != id && x.Slug == model.Slug);

        if (slugExists)
        {
            ModelState.AddModelError(
                nameof(model.Slug),
                "صفحه دیگری با این آدرس وجود دارد."
            );

            return View(model);
        }

        var wasPublished = page.IsPublished;

        page.Title = model.Title;
        page.Slug = model.Slug;
        page.Content = model.Content;
        page.IsPublished = model.IsPublished;
        page.UpdatedAt = DateTime.UtcNow;

        if (!wasPublished && model.IsPublished)
        {
            page.PublishedAt ??= DateTime.UtcNow;
        }
        else if (!model.IsPublished)
        {
            page.PublishedAt = null;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "صفحه با موفقیت ویرایش شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Pages/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var page = await _context.Pages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (page == null)
            return NotFound();

        return View(page);
    }

    // POST: Admin/Pages/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var page = await _context.Pages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (page == null)
            return NotFound();

        _context.Pages.Remove(page);

        await _context.SaveChangesAsync();

        TempData["Success"] = "صفحه با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}