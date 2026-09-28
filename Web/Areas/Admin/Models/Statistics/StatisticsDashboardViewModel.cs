namespace Web.Areas.Admin.Models.Statistics;

public class StatisticsDashboardViewModel
{
    public int TodayPageViews { get; set; }

    public int YesterdayPageViews { get; set; }

    public int Last7DaysPageViews { get; set; }

    public int Last30DaysPageViews { get; set; }

    public int TodayAudioViews { get; set; }

    public int TodayDownloads { get; set; }

    public int Last30DaysDownloads { get; set; }

    public List<DailyStatisticViewModel> DailyPageViews { get; set; } = new();

    public List<PopularPageViewModel> PopularPages { get; set; } = new();

    public List<PopularAudioViewModel> PopularAudios { get; set; } = new();
}


public class DailyStatisticViewModel
{
    public DateTime Date { get; set; }

    public int Count { get; set; }

    public string DisplayDate =>
        Date.ToString("MM/dd");
}


public class PopularPageViewModel
{
    public string Path { get; set; } = string.Empty;

    public int Count { get; set; }
}


public class PopularAudioViewModel
{
    public int AudioFileId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ViewCount { get; set; }
}