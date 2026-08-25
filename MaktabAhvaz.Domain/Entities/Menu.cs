namespace MaktabAhvaz.Domain.Entities;

public class Menu
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Url { get; set; }

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public int? ParentId { get; set; }

    public Menu? Parent { get; set; }
}