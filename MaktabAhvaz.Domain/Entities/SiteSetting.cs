namespace MaktabAhvaz.Domain.Entities;

public class SiteSetting
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }

    public string? Group { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; } = true;
}