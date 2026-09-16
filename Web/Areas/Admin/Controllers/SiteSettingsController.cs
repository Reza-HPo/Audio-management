using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SiteSettingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SiteSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _context.SiteSettings
            .AsNoTracking()
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Key)
            .ToListAsync();

        return View(settings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SiteSetting model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var keyExists = await _context.SiteSettings
            .AnyAsync(x => x.Key == model.Key);

        if (keyExists)
        {
            ModelState.AddModelError(
                nameof(model.Key),
                "تنظیمی با این کلید قبلاً وجود دارد."
            );

            return View(model);
        }

        model.Id = 0;

        _context.SiteSettings.Add(model);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تنظیم با موفقیت ایجاد شد.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var setting = await _context.SiteSettings
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setting == null)
            return NotFound();

        return View(setting);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SiteSetting model)
    {
        if (id != model.Id)
            return BadRequest();

        var setting = await _context.SiteSettings
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setting == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var keyExists = await _context.SiteSettings
            .AnyAsync(x => x.Id != id && x.Key == model.Key);

        if (keyExists)
        {
            ModelState.AddModelError(
                nameof(model.Key),
                "تنظیم دیگری با این کلید وجود دارد."
            );

            return View(model);
        }

        setting.Key = model.Key;
        setting.Value = model.Value;
        setting.Group = model.Group;
        setting.Description = model.Description;
        setting.IsPublic = model.IsPublic;

        await _context.SaveChangesAsync();

        TempData["Success"] = "تنظیم با موفقیت ویرایش شد.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var setting = await _context.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setting == null)
            return NotFound();

        return View(setting);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var setting = await _context.SiteSettings
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setting == null)
            return NotFound();

        _context.SiteSettings.Remove(setting);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تنظیم با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}