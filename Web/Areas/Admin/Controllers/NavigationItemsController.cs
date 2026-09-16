using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NavigationItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NavigationItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/NavigationItems
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _context.NavigationItems
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync();

        return View(items);
    }

    // GET: Admin/NavigationItems/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/NavigationItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NavigationItem model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.NavigationItems.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "آیتم منو با موفقیت ایجاد شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/NavigationItems/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.NavigationItems
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        return View(item);
    }

    // POST: Admin/NavigationItems/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NavigationItem model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var item = await _context.NavigationItems
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        item.Title = model.Title;
        item.Url = model.Url;
        item.Icon = model.Icon;
        item.DisplayOrder = model.DisplayOrder;
        item.IsActive = model.IsActive;
        item.OpenInNewTab = model.OpenInNewTab;

        await _context.SaveChangesAsync();

        TempData["Success"] = "آیتم منو با موفقیت ویرایش شد.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/NavigationItems/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.NavigationItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        return View(item);
    }

    // POST: Admin/NavigationItems/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.NavigationItems
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        _context.NavigationItems.Remove(item);
        await _context.SaveChangesAsync();

        TempData["Success"] = "آیتم منو با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}