using MaktabAhvaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: /
    // وضعیت API
    // =========================================================

    [HttpGet("~/")]
    public IActionResult GetApiStatus()
    {
        var html = """
    <!DOCTYPE html>
    <html lang="fa" dir="rtl">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />

        <title>MaktabAhvaz API</title>

        <style>
            * {
                box-sizing: border-box;
            }

            body {
                margin: 0;
                min-height: 100vh;
                font-family: Tahoma, Arial, sans-serif;
                background:
                    radial-gradient(circle at top right, #164e3b 0, transparent 35%),
                    radial-gradient(circle at bottom left, #0f766e 0, transparent 30%),
                    #071411;
                color: #ffffff;
                display: flex;
                align-items: center;
                justify-content: center;
                padding: 30px;
            }

            .container {
                width: 100%;
                max-width: 1000px;
            }

            .card {
                background: rgba(255, 255, 255, 0.08);
                border: 1px solid rgba(255, 255, 255, 0.12);
                backdrop-filter: blur(20px);
                -webkit-backdrop-filter: blur(20px);
                border-radius: 28px;
                padding: 45px;
                box-shadow: 0 25px 80px rgba(0, 0, 0, 0.35);
            }

            .header {
                text-align: center;
                margin-bottom: 40px;
            }

            .logo {
                width: 75px;
                height: 75px;
                margin: 0 auto 20px;
                border-radius: 22px;
                background: linear-gradient(135deg, #10b981, #059669);
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 34px;
                box-shadow: 0 12px 35px rgba(16, 185, 129, 0.3);
            }

            h1 {
                margin: 0 0 12px;
                font-size: 32px;
            }

            .subtitle {
                color: #b7c8c1;
                font-size: 15px;
            }

            .status {
                display: inline-flex;
                align-items: center;
                gap: 8px;
                margin-top: 20px;
                padding: 9px 16px;
                border-radius: 50px;
                background: rgba(16, 185, 129, 0.12);
                border: 1px solid rgba(16, 185, 129, 0.25);
                color: #6ee7b7;
                font-size: 14px;
            }

            .dot {
                width: 9px;
                height: 9px;
                border-radius: 50%;
                background: #10b981;
                box-shadow: 0 0 12px #10b981;
            }

            .endpoints {
                display: grid;
                grid-template-columns: repeat(2, 1fr);
                gap: 15px;
            }

            .endpoint {
                text-decoration: none;
                color: inherit;
                padding: 20px;
                border-radius: 18px;
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid rgba(255, 255, 255, 0.08);
                transition: 0.2s ease;
            }

            .endpoint:hover {
                transform: translateY(-3px);
                background: rgba(255, 255, 255, 0.09);
                border-color: rgba(16, 185, 129, 0.35);
            }

            .endpoint-title {
                font-size: 16px;
                font-weight: bold;
                margin-bottom: 8px;
            }

            .endpoint-url {
                direction: ltr;
                text-align: left;
                color: #6ee7b7;
                font-family: Consolas, monospace;
                font-size: 13px;
            }

            .footer {
                margin-top: 35px;
                padding-top: 20px;
                border-top: 1px solid rgba(255, 255, 255, 0.08);
                text-align: center;
                color: #82968e;
                font-size: 13px;
            }

            @media (max-width: 700px) {
                .card {
                    padding: 25px;
                }

                .endpoints {
                    grid-template-columns: 1fr;
                }

                h1 {
                    font-size: 26px;
                }
            }
        </style>
    </head>

    <body>

        <div class="container">
            <div class="card">

                <div class="header">

                    <div class="logo">
                        🎧
                    </div>

                    <h1>MaktabAhvaz API</h1>

                    <div class="subtitle">
                        رابط برنامه‌نویسی سامانه مکتب اهواز
                    </div>

                    <div class="status">
                        <span class="dot"></span>
                        API در حال اجراست
                    </div>

                </div>

                <div class="endpoints">

                    <a class="endpoint" href="/api/home">
                        <div class="endpoint-title">
                            🏠 صفحه اصلی
                        </div>
                        <div class="endpoint-url">
                            GET /api/home
                        </div>
                    </a>

                    <a class="endpoint" href="/api/audios">
                        <div class="endpoint-title">
                            🎧 صوت‌ها
                        </div>
                        <div class="endpoint-url">
                            GET /api/audios
                        </div>
                    </a>

                    <a class="endpoint" href="/api/audios/latest">
                        <div class="endpoint-title">
                            🆕 آخرین صوت‌ها
                        </div>
                        <div class="endpoint-url">
                            GET /api/audios/latest
                        </div>
                    </a>

                    <a class="endpoint" href="/api/speakers">
                        <div class="endpoint-title">
                            👤 سخنران‌ها
                        </div>
                        <div class="endpoint-url">
                            GET /api/speakers
                        </div>
                    </a>

                    <a class="endpoint" href="/api/categories">
                        <div class="endpoint-title">
                            🏷️ دسته‌بندی‌ها
                        </div>
                        <div class="endpoint-url">
                            GET /api/categories
                        </div>
                    </a>

                </div>

                <div class="footer">
                    MaktabAhvaz API · Version 1.0
                </div>

            </div>
        </div>

    </body>
    </html>
    """;

        return Content(html, "text/html; charset=utf-8");
    }


    // =========================================================
    // GET: /api/home
    // اطلاعات مورد نیاز صفحه اصلی
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetHome()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        // =========================================================
        // آخرین فایل‌های صوتی
        // =========================================================

        var latestAudios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(10)
            .Select(a => new
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,

                FileUrl = string.IsNullOrWhiteSpace(a.FileName)
                    ? null
                    : $"{baseUrl}{a.FileName}",

                CoverImageUrl = string.IsNullOrWhiteSpace(a.CoverImageUrl)
                    ? null
                    : $"{baseUrl}{a.CoverImageUrl}",

                Duration = a.Duration,
                PublishedAt = a.PublishedAt,

                Speaker = a.Speaker == null
                    ? null
                    : new
                    {
                        Id = a.Speaker.Id,
                        Name = a.Speaker.Name,
                        ImageUrl = string.IsNullOrWhiteSpace(a.Speaker.ImageUrl)
                            ? null
                            : $"{baseUrl}{a.Speaker.ImageUrl}"
                    }
            })
            .ToListAsync();

        // =========================================================
        // سخنران‌ها
        // =========================================================

        var speakers = await _context.Speakers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                Id = s.Id,
                Name = s.Name,

                ImageUrl = string.IsNullOrWhiteSpace(s.ImageUrl)
                    ? null
                    : $"{baseUrl}{s.ImageUrl}"
            })
            .ToListAsync();

        // =========================================================
        // دسته‌بندی‌ها
        // =========================================================

        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        // =========================================================
        // خروجی نهایی
        // =========================================================

        return Ok(new
        {
            latestAudios,
            speakers,
            categories
        });
    }
}