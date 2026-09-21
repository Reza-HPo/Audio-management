using MaktabAhvaz.Domain.Entities;

namespace Web.Models.Speakers;

public class SpeakerListViewModel
{
    public List<SpeakerItemViewModel> Speakers { get; set; } = new();
}

public class SpeakerItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public int AudioCount { get; set; }
}