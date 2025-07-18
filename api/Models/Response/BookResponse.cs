namespace PersonalLibrary.Models.Response;

public class BookResponse
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? SortTitle { get; set; }
    public string? Subtitle { get; set; }
    public required string Author { get; set; }
    public string? SortAuthor { get; set; }
    public string? Translator { get; set; }
    public string? SortTranslator { get; set; }
    public required string Language { get; set; }
    public string? OriginalLanguage { get; set; }
    public string? Collection { get; set; }
    public int PublicationYear { get; set; }
    public int EditionPublicationYear { get; set; }
    public bool Read { get; set; }
    public string? Notes { get; set; }
}