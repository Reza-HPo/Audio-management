using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models.Statistics;
using Web.Services;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
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
        var dailyUniqueVisitors =
            await _statisticsService
                .GetLast7DaysUniqueVisitorsDailyAsync();

        var popularPages =
            await _statisticsService
                .GetPopularPagesAsync(10);

        var popularAudios =
            await _statisticsService
                .GetPopularAudiosAsync(10);

        var model = new StatisticsDashboardViewModel
        {
            TodayUniqueVisitors =
                await _statisticsService
                    .GetTodayUniqueVisitorsAsync(),

            YesterdayUniqueVisitors =
                await _statisticsService
                    .GetYesterdayUniqueVisitorsAsync(),

            Last7DaysUniqueVisitors =
                await _statisticsService
                    .GetLast7DaysUniqueVisitorsAsync(),

            Last30DaysUniqueVisitors =
                await _statisticsService
                    .GetLast30DaysUniqueVisitorsAsync(),

            TodayPageViews =
                await _statisticsService
                    .GetTodayPageViewCountAsync(),

            TodayAudioViews =
                await _statisticsService
                    .GetTodayAudioViewsAsync(),

            TodayDownloads =
                await _statisticsService
                    .GetTodayDownloadsAsync(),

            Last30DaysDownloads =
                await _statisticsService
                    .GetLast30DaysDownloadsAsync(),

            DailyPageViews = dailyUniqueVisitors
                .Select(x => new DailyStatisticViewModel
                {
                    Date = x.Date,
                    Count = x.Count
                })
                .ToList(),

            PopularPages = popularPages
                .Select(x => new PopularPageViewModel
                {
                    Path = x.Path,
                    Count = x.Count
                })
                .ToList(),

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