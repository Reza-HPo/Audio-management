using Microsoft.EntityFrameworkCore;
using MaktabAhvaz.Infrastructure.Data;
using MaktabAhvaz.Domain.Entities;

namespace Web.Services;

public class StatisticsDashboardService
{
    private readonly ApplicationDbContext _context;

    public StatisticsDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // SITE
    // =========================================================

    public async Task<int> GetTodayPageViewsAsync()
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

    public async Task<int> GetYesterdayPageViewsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= yesterday &&
                s.CreatedAt < today);
    }

    public async Task<int> GetLast7DaysPageViewCountAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-6);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate);
    }

    public async Task<int> GetLast30DaysPageViewsAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-29);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        return await _context.SiteStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate);
    }

    // =========================================================
    // AUDIO
    // =========================================================

    public async Task<int> GetTodayAudioViewsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.AudioStatistics
            .AsNoTracking()
            .CountAsync(s =>
                s.EventType == AudioStatisticEventType.View &&
                s.CreatedAt >= today &&
                s.CreatedAt < tomorrow);
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

    // =========================================================
    // DAILY PAGE VIEWS
    // =========================================================

    public async Task<List<DailyStatisticItem>> GetLast7DaysPageViewsAsync()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-6);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        var data = await _context.SiteStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == SiteStatisticEventType.PageView &&
                s.CreatedAt >= startDate &&
                s.CreatedAt < endDate)
            .GroupBy(s => s.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var result = new List<DailyStatisticItem>();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
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

    // =========================================================
    // POPULAR PAGES
    // =========================================================

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

    // =========================================================
    // POPULAR AUDIO
    // =========================================================

    public async Task<List<PopularAudioStatisticItem>> GetPopularAudiosAsync(
        int take = 10)
    {
        return await _context.AudioStatistics
            .AsNoTracking()
            .Where(s =>
                s.EventType == AudioStatisticEventType.View)
            .GroupBy(s => s.AudioFileId)
            .Select(g => new PopularAudioStatisticItem
            {
                AudioFileId = g.Key,
                ViewCount = g.Count()
            })
            .OrderByDescending(x => x.ViewCount)
            .Take(take)
            .Join(
                _context.AudioFiles.AsNoTracking(),
                statistic => statistic.AudioFileId,
                audio => audio.Id,
                (statistic, audio) => new PopularAudioStatisticItem
                {
                    AudioFileId = audio.Id,
                    Title = audio.Title,
                    ViewCount = statistic.ViewCount
                })
            .ToListAsync();
    }
}


// =========================================================
// DTOs
// =========================================================

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