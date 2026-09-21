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

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <meta name="theme-color"
          content="#071411" />

    <meta name="description"
          content="API سامانه مجالس اهواز" />

    <title>مجالس اهواز | API</title>

    <style>

        /* =====================================================
           RESET
           ===================================================== */

        * {
            box-sizing: border-box;
        }

        html {
            scroll-behavior: smooth;
        }

        body {
            margin: 0;
            min-height: 100vh;

            font-family:
                Tahoma,
                "Segoe UI",
                Arial,
                sans-serif;

            color: #ffffff;

            background:
                radial-gradient(
                    circle at 85% 10%,
                    rgba(16, 185, 129, 0.20),
                    transparent 30%
                ),
                radial-gradient(
                    circle at 10% 85%,
                    rgba(20, 184, 166, 0.13),
                    transparent 28%
                ),
                linear-gradient(
                    145deg,
                    #04100d 0%,
                    #071713 45%,
                    #06110e 100%
                );

            display: flex;
            align-items: center;
            justify-content: center;

            padding: 32px 18px;

            overflow-x: hidden;
        }


        /* =====================================================
           BACKGROUND EFFECTS
           ===================================================== */

        body::before,
        body::after {
            content: "";
            position: fixed;

            width: 260px;
            height: 260px;

            border-radius: 50%;

            filter: blur(80px);

            pointer-events: none;

            opacity: .35;
        }

        body::before {
            top: -100px;
            right: -80px;

            background: #10b981;
        }

        body::after {
            bottom: -120px;
            left: -90px;

            background: #0f766e;
        }


        /* =====================================================
           MAIN
           ===================================================== */

        .page {
            width: 100%;
            max-width: 1080px;

            position: relative;
            z-index: 2;
        }


        /* =====================================================
           CARD
           ===================================================== */

        .glass-card {
            position: relative;

            overflow: hidden;

            border-radius: 32px;

            border: 1px solid rgba(255, 255, 255, .11);

            background:
                linear-gradient(
                    145deg,
                    rgba(255,255,255,.095),
                    rgba(255,255,255,.035)
                );

            box-shadow:
                0 30px 90px rgba(0,0,0,.40),
                inset 0 1px 0 rgba(255,255,255,.08);

            backdrop-filter: blur(28px);
            -webkit-backdrop-filter: blur(28px);
        }


        /* =====================================================
           TOP GLOW
           ===================================================== */

        .glass-card::before {
            content: "";

            position: absolute;

            top: -180px;
            right: 15%;

            width: 450px;
            height: 300px;

            background:
                radial-gradient(
                    circle,
                    rgba(16,185,129,.16),
                    transparent 70%
                );

            pointer-events: none;
        }


        /* =====================================================
           HEADER
           ===================================================== */

        .header {
            position: relative;

            text-align: center;

            padding: 55px 30px 40px;
        }


        /* =====================================================
           LOGO
           ===================================================== */

        .logo-wrapper {
            position: relative;

            width: 92px;
            height: 92px;

            margin: 0 auto 24px;

            display: flex;
            align-items: center;
            justify-content: center;
        }

        .logo-glow {
            position: absolute;

            inset: -12px;

            border-radius: 30px;

            background:
                radial-gradient(
                    circle,
                    rgba(16,185,129,.28),
                    transparent 70%
                );

            filter: blur(12px);
        }

        .logo {
            position: relative;

            width: 82px;
            height: 82px;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 25px;

            background:
                linear-gradient(
                    135deg,
                    #10b981,
                    #059669
                );

            border: 1px solid rgba(255,255,255,.16);

            box-shadow:
                0 18px 45px rgba(16,185,129,.28),
                inset 0 1px 0 rgba(255,255,255,.22);

            font-size: 39px;
        }


        /* =====================================================
           TITLE
           ===================================================== */

        h1 {
            margin: 0;

            font-size: clamp(30px, 5vw, 44px);

            font-weight: 800;

            letter-spacing: -.8px;
        }

        .brand {
            color: #6ee7b7;
        }

        .subtitle {
            margin-top: 14px;

            color: #a9bbb5;

            font-size: 15px;

            line-height: 2;
        }


        /* =====================================================
           STATUS
           ===================================================== */

        .status {
            display: inline-flex;

            align-items: center;

            gap: 9px;

            margin-top: 22px;

            padding: 10px 17px;

            border-radius: 999px;

            background:
                rgba(16,185,129,.09);

            border:
                1px solid rgba(16,185,129,.22);

            color: #6ee7b7;

            font-size: 13px;

            box-shadow:
                0 8px 30px rgba(16,185,129,.06);
        }

        .dot {
            width: 8px;
            height: 8px;

            border-radius: 50%;

            background: #10b981;

            box-shadow:
                0 0 0 4px rgba(16,185,129,.10),
                0 0 18px rgba(16,185,129,.7);
        }


        /* =====================================================
           API INFO
           ===================================================== */

        .api-info {
            margin: 0 30px 30px;

            padding: 18px 20px;

            display: flex;

            align-items: center;

            justify-content: space-between;

            gap: 20px;

            border-radius: 18px;

            background:
                rgba(0,0,0,.18);

            border:
                1px solid rgba(255,255,255,.07);
        }

        .api-label {
            color: #81968e;

            font-size: 12px;

            margin-bottom: 6px;
        }

        .api-url {
            direction: ltr;

            text-align: left;

            font-family:
                Consolas,
                "Courier New",
                monospace;

            color: #b7f7d9;

            font-size: 13px;

            word-break: break-all;
        }

        .version {
            flex-shrink: 0;

            padding: 7px 11px;

            border-radius: 9px;

            background:
                rgba(255,255,255,.06);

            color: #91aaa1;

            font-size: 11px;
        }


        /* =====================================================
           SECTION
           ===================================================== */

        .section {
            padding: 0 30px 32px;
        }

        .section-header {
            display: flex;

            align-items: center;

            justify-content: space-between;

            margin-bottom: 16px;
        }

        .section-title {
            margin: 0;

            font-size: 16px;

            font-weight: 700;
        }

        .section-description {
            color: #748a82;

            font-size: 11px;
        }


        /* =====================================================
           ENDPOINT GRID
           ===================================================== */

        .endpoints {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 14px;
        }


        /* =====================================================
           ENDPOINT
           ===================================================== */

        .endpoint {
            position: relative;

            display: block;

            text-decoration: none;

            color: inherit;

            padding: 20px;

            border-radius: 19px;

            background:
                rgba(255,255,255,.045);

            border:
                1px solid rgba(255,255,255,.075);

            transition:
                transform .22s ease,
                background .22s ease,
                border-color .22s ease,
                box-shadow .22s ease;
        }

        .endpoint:hover {
            transform: translateY(-4px);

            background:
                rgba(16,185,129,.075);

            border-color:
                rgba(16,185,129,.30);

            box-shadow:
                0 15px 35px rgba(0,0,0,.18);
        }


        /* =====================================================
           ENDPOINT TOP
           ===================================================== */

        .endpoint-top {
            display: flex;

            align-items: center;

            gap: 13px;

            margin-bottom: 13px;
        }

        .endpoint-icon {
            width: 42px;
            height: 42px;

            flex-shrink: 0;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 13px;

            background:
                rgba(16,185,129,.10);

            border:
                1px solid rgba(16,185,129,.13);

            font-size: 19px;
        }

        .endpoint-title {
            font-size: 14px;

            font-weight: 700;
        }

        .endpoint-description {
            margin-top: 4px;

            color: #81958d;

            font-size: 11px;
        }


        /* =====================================================
           ENDPOINT URL
           ===================================================== */

        .endpoint-url {
            direction: ltr;

            text-align: left;

            padding: 9px 11px;

            border-radius: 9px;

            background:
                rgba(0,0,0,.18);

            color: #62dca7;

            font-family:
                Consolas,
                monospace;

            font-size: 11px;

            overflow: hidden;

            white-space: nowrap;

            text-overflow: ellipsis;
        }


        /* =====================================================
           FOOTER
           ===================================================== */

        .footer {
            margin: 0 30px;

            padding: 23px 0 28px;

            border-top:
                1px solid rgba(255,255,255,.07);

            display: flex;

            align-items: center;

            justify-content: space-between;

            gap: 15px;

            color: #61756d;

            font-size: 11px;
        }

        .footer-brand {
            color: #9cafaa;

            font-weight: 700;
        }

        .footer-note {
            direction: ltr;

            font-family: Consolas, monospace;

            opacity: .7;
        }


        /* =====================================================
           MOBILE
           ===================================================== */

        @media (max-width: 720px) {

            body {
                padding: 15px;
            }

            .glass-card {
                border-radius: 24px;
            }

            .header {
                padding:
                    40px 20px 30px;
            }

            .logo {
                width: 72px;
                height: 72px;

                border-radius: 21px;

                font-size: 33px;
            }

            .logo-wrapper {
                width: 78px;
                height: 78px;
            }

            .api-info {
                margin:
                    0 18px 25px;

                flex-direction: column;

                align-items: flex-start;
            }

            .section {
                padding:
                    0 18px 25px;
            }

            .endpoints {
                grid-template-columns: 1fr;
            }

            .footer {
                margin: 0 18px;

                flex-direction: column;

                text-align: center;
            }
        }


        /* =====================================================
           SMALL MOBILE
           ===================================================== */

        @media (max-width: 400px) {

            h1 {
                font-size: 28px;
            }

            .subtitle {
                font-size: 13px;
            }

            .endpoint {
                padding: 17px;
            }
        }

    </style>
</head>


<body>

    <main class="page">

        <section class="glass-card">

            <!-- =================================================
                 HEADER
                 ================================================= -->

            <header class="header">

                <div class="logo-wrapper">

                    <div class="logo-glow"></div>

                    <div class="logo">
                        🎧
                    </div>

                </div>

                <h1>
                    <span class="brand">
                        مجالس اهواز
                    </span>
                    API
                </h1>

                <div class="subtitle">
                    رابط برنامه‌نویسی سامانه مجالس اهواز
                    <br />
                    دسترسی یکپارچه به آرشیو صوتی، سخنرانان و دسته‌بندی‌ها
                </div>

                <div class="status">
                    <span class="dot"></span>
                    سرویس با موفقیت در حال اجراست
                </div>

            </header>


            <!-- =================================================
                 API INFO
                 ================================================= -->

            <div class="api-info">

                <div>

                    <div class="api-label">
                        آدرس پایه API
                    </div>

                    <div class="api-url">
                        https://api.maktabahwaz.ir
                    </div>

                </div>

                <div class="version">
                    API v1.0
                </div>

            </div>


            <!-- =================================================
                 ENDPOINTS
                 ================================================= -->

            <section class="section">

                <div class="section-header">

                    <h2 class="section-title">
                        سرویس‌های API
                    </h2>

                    <span class="section-description">
                        Endpoints
                    </span>

                </div>


                <div class="endpoints">


                    <!-- HOME -->

                    <a class="endpoint"
                       href="/api/home">

                        <div class="endpoint-top">

                            <div class="endpoint-icon">
                                🏠
                            </div>

                            <div>

                                <div class="endpoint-title">
                                    صفحه اصلی
                                </div>

                                <div class="endpoint-description">
                                    اطلاعات اصلی سامانه
                                </div>

                            </div>

                        </div>

                        <div class="endpoint-url">
                            GET /api/home
                        </div>

                    </a>


                    <!-- AUDIOS -->

                    <a class="endpoint"
                       href="/api/audios">

                        <div class="endpoint-top">

                            <div class="endpoint-icon">
                                🎧
                            </div>

                            <div>

                                <div class="endpoint-title">
                                    آرشیو صوتی
                                </div>

                                <div class="endpoint-description">
                                    دریافت و جستجوی صوت‌ها
                                </div>

                            </div>

                        </div>

                        <div class="endpoint-url">
                            GET /api/audios
                        </div>

                    </a>


                    <!-- LATEST -->

                    <a class="endpoint"
                       href="/api/audios/latest">

                        <div class="endpoint-top">

                            <div class="endpoint-icon">
                                ✨
                            </div>

                            <div>

                                <div class="endpoint-title">
                                    آخرین صوت‌ها
                                </div>

                                <div class="endpoint-description">
                                    جدیدترین فایل‌های منتشرشده
                                </div>

                            </div>

                        </div>

                        <div class="endpoint-url">
                            GET /api/audios/latest
                        </div>

                    </a>


                    <!-- SPEAKERS -->

                    <a class="endpoint"
                       href="/api/speakers">

                        <div class="endpoint-top">

                            <div class="endpoint-icon">
                                👤
                            </div>

                            <div>

                                <div class="endpoint-title">
                                    سخنران‌ها
                                </div>

                                <div class="endpoint-description">
                                    فهرست سخنرانان سامانه
                                </div>

                            </div>

                        </div>

                        <div class="endpoint-url">
                            GET /api/speakers
                        </div>

                    </a>


                    <!-- CATEGORIES -->

                    <a class="endpoint"
                       href="/api/categories">

                        <div class="endpoint-top">

                            <div class="endpoint-icon">
                                🏷️
                            </div>

                            <div>

                                <div class="endpoint-title">
                                    دسته‌بندی‌ها
                                </div>

                                <div class="endpoint-description">
                                    دسته‌بندی آرشیو صوتی
                                </div>

                            </div>

                        </div>

                        <div class="endpoint-url">
                            GET /api/categories
                        </div>

                    </a>


                </div>

            </section>


            <!-- =================================================
                 FOOTER
                 ================================================= -->

            <footer class="footer">

                <div>
                    <span class="footer-brand">
                        مجالس اهواز
                    </span>

                    <span>
                        · سامانه آرشیو صوتی
                    </span>
                </div>

                <div class="footer-note">
                    REST API · v1.0
                </div>

            </footer>

        </section>

    </main>

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
                    : a.FileName.StartsWith(
                        "http",
                        StringComparison.OrdinalIgnoreCase)
                        ? a.FileName
                        : $"{baseUrl}{a.FileName}",

                CoverImageUrl = string.IsNullOrWhiteSpace(a.CoverImageUrl)
                    ? null
                    : a.CoverImageUrl.StartsWith(
                        "http",
                        StringComparison.OrdinalIgnoreCase)
                        ? a.CoverImageUrl
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
                            : a.Speaker.ImageUrl.StartsWith(
                                "http",
                                StringComparison.OrdinalIgnoreCase)
                                ? a.Speaker.ImageUrl
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
                    : s.ImageUrl.StartsWith(
                        "http",
                        StringComparison.OrdinalIgnoreCase)
                        ? s.ImageUrl
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