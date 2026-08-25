namespace MaktabAhvaz.Domain.Entities;

public class Page
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}