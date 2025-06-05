namespace PersonalLibrary.Models;

public class Book
{
    public int BookId { get; set; }
    public int Isbn { get; set; }
    public string? Title { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public string? Translator { get; set; } = null;
    public string? OriginalLanguage { get; set; }
    public int CollectionId { get; set; }
    public Collection? Collection { get; set; }
    public int PublicationYear { get; set; }
    public int EditionPublicationYear { get; set; }
    public bool Read { get; set; }
    public string? Notes { get; set; }
}
