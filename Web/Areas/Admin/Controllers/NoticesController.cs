using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NoticesController : Controller
{
    private readonly ApplicationDbContext _context;

    public NoticesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Notices
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var notices = await _context.Notices
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(notices);
    }

    // GET: Admin/Notices/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Notices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Notice model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Id = 0;
        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = null;

        if (model.IsPublished && model.PublishedAt == null)
            model.PublishedAt = DateTime.UtcNow;

        if (!model.IsPublished)
            model.PublishedAt = null;

        _context.Notices.Add(model);

        await _context.SaveChangesAsync();

        TempData["Success"] = "اطلاعیه با موفقیت ایجاد شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Notices/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var notice = await _context.Notices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (notice == null)
            return NotFound();

        return View(notice);
    }

    // POST: Admin/Notices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Notice model)
    {
        if (id != model.Id)
            return BadRequest();

        var notice = await _context.Notices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (notice == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var wasPublished = notice.IsPublished;

        notice.Title = model.Title;
        notice.Summary = model.Summary;
        notice.Content = model.Content;
        notice.ImageUrl = model.ImageUrl;
        notice.IsPublished = model.IsPublished;
        notice.UpdatedAt = DateTime.UtcNow;

        if (!wasPublished && model.IsPublished)
        {
            notice.PublishedAt ??= DateTime.UtcNow;
        }
        else if (!model.IsPublished)
        {
            notice.PublishedAt = null;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "اطلاعیه با موفقیت ویرایش شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Notices/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var notice = await _context.Notices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (notice == null)
            return NotFound();

        return View(notice);
    }

    // POST: Admin/Notices/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var notice = await _context.Notices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (notice == null)
            return NotFound();

        _context.Notices.Remove(notice);

        await _context.SaveChangesAsync();

        TempData["Success"] = "اطلاعیه با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}