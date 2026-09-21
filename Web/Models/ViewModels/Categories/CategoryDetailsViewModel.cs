namespace Web.Models.Categories;

public class CategoryDetailsViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public List<CategoryAudioViewModel> Audios { get; set; }
        = new();
}