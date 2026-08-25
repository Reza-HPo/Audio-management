namespace MaktabAhvaz.Domain.Entities;

public class AudioCategory
{
    public int AudioFileId { get; set; }

    public AudioFile AudioFile { get; set; } = null!;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}