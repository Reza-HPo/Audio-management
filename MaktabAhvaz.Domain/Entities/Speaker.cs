namespace MaktabAhvaz.Domain.Entities;

public class Speaker
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}