using PersonalLibrary.Models.Database;
using PersonalLibrary.Models.Request;
using PersonalLibrary.Models.Response;
using PersonalLibrary.Repositories;

namespace PersonalLibrary.Services;

public interface IBooksService
{
    Task<List<BookResponse>> GetAllBooksResponse();
    Task Add(BookRequest newBook);
    Task UpdateReadStatus(string isbn);
}

public class BooksService : IBooksService
{
    private readonly IBooksRepo _booksRepo;
    private readonly IAuthorsRepo _authorsRepo;
    public BooksService(IBooksRepo booksRepo, IAuthorsRepo authorsRepo, ICollectionsRepo collectionsRepo)
    {
        _booksRepo = booksRepo;
        _authorsRepo = authorsRepo;
    }

    public async Task<List<BookResponse>> GetAllBooksResponse()
    {
        var allBooks = await _booksRepo.GetAll();
        return [.. allBooks.Select(b => new BookResponse
        {
            Id = b.Id,
            Title = b.Title,
            SortTitle = RemoveLeadingArticle(b.Title),
            Subtitle = b.Subtitle,
            Author = b.Author!.Name,
            SortAuthor = b.Author.Name.Split(' ').Last(),
            Translator = b.Translator,
            SortTranslator = b.Translator?.Split(' ').Last(),
            Language = b.Language,
            OriginalLanguage = b.OriginalLanguage,
            Collection = b.Collection?.Name,
            PublicationYear = b.PublicationYear,
            EditionPublicationYear = b.EditionPublicationYear,
            Read = b.Read,
            Notes = b.Notes
        })];
    }

    public async Task Add(BookRequest newBook)
    {
        var author = await _authorsRepo.GetAuthorByName(newBook.Author);
        if (author == null)
        {
            await _authorsRepo.AddAuthor(newBook.Author);
        }
        author = await _authorsRepo.GetAuthorByName(newBook.Author);
        if (author != null)
        {
            Book book = new()
            {
                Id = newBook.Isbn,
                Title = newBook.Title,
                Author = author,
                Translator = newBook.Translator,
                Language = newBook.Language,
                OriginalLanguage = newBook.OriginalLanguage,
                CollectionId = newBook.CollectionId,
                PublicationYear = newBook.PublicationYear,
                EditionPublicationYear = newBook.EditionPublicationYear,
                Read = newBook.Read,
                Notes = newBook.Notes
            };
            await _booksRepo.Add(book);
        }
    }

    public async Task UpdateReadStatus(string isbn)
    {
        var book = await _booksRepo.Get(isbn);
        if (book.Read == true)
        {
            book.Read = false;
        }
        else
        {
            book.Read = true;
        }
        await _booksRepo.Update(book);
    }

    public static string RemoveLeadingArticle(string title)
    {
        var articles = new[] { "The ", "A ", "An " };
        foreach (var article in articles)
        {
            if (title.StartsWith(article, StringComparison.OrdinalIgnoreCase))
            {
                return title.Substring(article.Length);
            }
        }
        return title;
    }
}