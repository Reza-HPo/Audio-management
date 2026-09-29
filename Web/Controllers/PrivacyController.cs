using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class PrivacyController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "حریم خصوصی";

        return View();
    }
}