using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "ارتباط با ما";

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(
        string name,
        string email,
        string subject,
        string message)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(message))
        {
            ModelState.AddModelError(
                string.Empty,
                "لطفاً اطلاعات الزامی را کامل کنید.");

            return View();
        }

        // فعلاً پیام در دیتابیس ذخیره نمی‌شود.
        // در مرحله بعد این بخش را به سیستم پیام‌های سایت متصل می‌کنیم.

        TempData["ContactSuccess"] =
            "پیام شما با موفقیت ثبت شد. از اینکه با مجالس اهواز در ارتباط هستید سپاسگزاریم.";

        return RedirectToAction(nameof(Index));
    }
}