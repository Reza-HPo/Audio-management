using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models.Statistics;
using Web.Services;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
public class StatisticsController : Controller
{
    private readonly StatisticsDashboardService _statisticsService;

    public StatisticsController(
        StatisticsDashboardService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // دریافت آمار نمودار ۷ روز اخیر
        var dailyPageViews =
            await _statisticsService.GetLast7DaysPageViewsAsync();

        // صفحات پربازدید
        var popularPages =
            await _statisticsService.GetPopularPagesAsync(10);

        // فایل‌های صوتی پربازدید
        var popularAudios =
            await _statisticsService.GetPopularAudiosAsync(10);

        var model = new StatisticsDashboardViewModel
        {
            // آمار بازدید سایت
            TodayPageViews =
                await _statisticsService.GetTodayPageViewsAsync(),

            YesterdayPageViews =
                await _statisticsService.GetYesterdayPageViewsAsync(),

            Last7DaysPageViews =
                dailyPageViews.Sum(x => x.Count),

            Last30DaysPageViews =
                await _statisticsService.GetLast30DaysPageViewsAsync(),

            // آمار فایل‌های صوتی
            TodayAudioViews =
                await _statisticsService.GetTodayAudioViewsAsync(),

            TodayDownloads =
                await _statisticsService.GetTodayDownloadsAsync(),

            Last30DaysDownloads =
                await _statisticsService.GetLast30DaysDownloadsAsync(),

            // نمودار بازدید روزانه
            DailyPageViews = dailyPageViews
                .Select(x => new DailyStatisticViewModel
                {
                    Date = x.Date,
                    Count = x.Count
                })
                .ToList(),

            // صفحات پربازدید
            PopularPages = popularPages
                .Select(x => new PopularPageViewModel
                {
                    Path = x.Path,
                    Count = x.Count
                })
                .ToList(),

            // فایل‌های صوتی پربازدید
            PopularAudios = popularAudios
                .Select(x => new PopularAudioViewModel
                {
                    AudioFileId = x.AudioFileId,
                    Title = x.Title,
                    ViewCount = x.ViewCount
                })
                .ToList()
        };

        return View(model);
    }
}