namespace MaktabAhvaz.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public ICollection<AudioCategory> AudioCategories { get; set; }
    = new List<AudioCategory>();
}