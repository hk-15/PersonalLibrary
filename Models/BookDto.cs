namespace PersonalLibrary.Models;

public class BookDto
{
    public int BookDtoId { get; set; }
    public string? Isbn { get; set; }
    public required string Title { get; set; }
    public required string AuthorName { get; set; }
    public string? Translator { get; set; } = null;
    public string? OriginalLanguage { get; set; }
    public required string Collection { get; set; }
    public int PublicationYear { get; set; }
    public int EditionPublicationYear { get; set; }
    public bool Read { get; set; }
    public string? Notes { get; set; }
}
