using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AboutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "درباره ما";

        return View();
    }
}