using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class TermsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "قوانین و مقررات";

        return View();
    }
}