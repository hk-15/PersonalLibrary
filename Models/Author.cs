namespace PersonalLibrary.Models;

public class Author
{
    public Author()
    {
        Books = new List<Book>();
    }
    public int? AuthorId { get; set; }
    public string? Name { get; set; }
    public IList<Book>? Books { get; set; }
}