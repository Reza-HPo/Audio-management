namespace Web.Models.Categories;

public class CategoryListViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public int AudioCount { get; set; }
}