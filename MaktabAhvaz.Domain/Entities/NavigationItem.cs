namespace MaktabAhvaz.Domain.Entities;

public class NavigationItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool OpenInNewTab { get; set; } = false;
}