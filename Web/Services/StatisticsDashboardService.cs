using Microsoft.EntityFrameworkCore;
using MaktabAhvaz.Domain.Entities;
using MaktabAhvaz.Infrastructure.Data;

namespace Web.Services;

public class StatisticsDashboardService
{
    private readonly ApplicationDbContext _context;

    public StatisticsDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTodayUniqueVisitorsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= today &&
                s.CreatedAt < tomorrow &&
                s.VisitorId != null)
            .Select(s => s.VisitorId!)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetYesterdayUniqueVisitorsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= yesterday &&
                s.CreatedAt < today &&
                s.VisitorId != null)
            .Select(s => s.VisitorId!)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetLast7DaysUniqueVisitorsAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-6);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate &&
                s.VisitorId != null)
            .Select(s => s.VisitorId!)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetLast30DaysUniqueVisitorsAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-29);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate &&
                s.VisitorId != null)
            .Select(s => s.VisitorId!)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetTodayPageViewCountAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= today &&
                s.CreatedAt < tomorrow);
    }

    /// <summary>
    /// تعداد بازدیدکنندگان یکتای صوت‌ها در امروز
    /// </summary>
    public async Task<int> GetTodayAudioViewsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.AudioStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == AudioStatisticEventType.View &&
                s.CreatedAt >= today &&
                s.CreatedAt < tomorrow &&
                s.VisitorId != null)
            .Select(s => s.VisitorId!)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetTodayDownloadsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.AudioStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == AudioStatisticEventType.Download &&
                s.CreatedAt >= today &&
                s.CreatedAt < tomorrow);
    }

    public async Task<int> GetLast30DaysDownloadsAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-29);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        return await _context.AudioStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == AudioStatisticEventType.Download &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate);
    }

    public async Task<List<DailyStatisticItem>> GetLast7DaysUniqueVisitorsDailyAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-6);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        var data = await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate &&
                s.VisitorId != null)
            .GroupBy(s => new
            {
                Date = s.CreatedAt.Date,
                s.VisitorId
            })
            .Select(g => new
            {
                Date = g.Key.Date,
                VisitorId = g.Key.VisitorId
            })
            .GroupBy(x => x.Date)
            .Select(g => new DailyStatisticItem
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var result = new List<DailyStatisticItem>();

        for (var date = startDate;
             date < endDate;
             date = date.AddDays(1))
        {
            var item = data.FirstOrDefault(x => x.Date == date);

            result.Add(new DailyStatisticItem
            {
                Date = date,
                Count = item?.Count ?? 0
            });
        }

        return result;
    }

    public async Task<List<PopularPageStatisticItem>> GetPopularPagesAsync(
        int take = 10)
    {
        return await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView)
            .GroupBy(s => s.Path)
            .Select(g => new PopularPageStatisticItem
            {
                Path = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// محبوب‌ترین صوت‌ها بر اساس تعداد بازدیدکنندگان یکتا
    /// </summary>
    public async Task<List<PopularAudioStatisticItem>> GetPopularAudiosAsync(
    int take = 10)
    {
        var statistics = await _context.AudioStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == AudioStatisticEventType.View &&
                s.VisitorId != null)
            .GroupBy(s => new
            {
                s.AudioFileId,
                s.VisitorId
            })
            .Select(g => new
            {
                AudioFileId = g.Key.AudioFileId,
                VisitorId = g.Key.VisitorId
            })
            .GroupBy(x => x.AudioFileId)
            .Select(g => new
            {
                AudioFileId = g.Key,
                ViewCount = g.Count()
            })
            .OrderByDescending(x => x.ViewCount)
            .Take(take)
            .ToListAsync();

        var audioIds = statistics
            .Select(x => x.AudioFileId)
            .ToList();

        var audios = await _context.AudioFiles
            .AsNoTracking()
            .Where(a => audioIds.Contains(a.Id))
            .ToListAsync();

        return statistics
            .Join(
                audios,
                statistic => statistic.AudioFileId,
                audio => audio.Id,
                (statistic, audio) => new PopularAudioStatisticItem
                {
                    AudioFileId = audio.Id,
                    Title = audio.Title,
                    ViewCount = statistic.ViewCount
                })
            .ToList();
    }
}

public class DailyStatisticItem
{
    public DateTime Date { get; set; }

    public int Count { get; set; }
}

public class PopularPageStatisticItem
{
    public string Path { get; set; } = string.Empty;

    public int Count { get; set; }
}

public class PopularAudioStatisticItem
{
    public int AudioFileId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ViewCount { get; set; }
}