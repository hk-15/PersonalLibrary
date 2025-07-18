using Microsoft.AspNetCore.Mvc;
using PersonalLibrary.Models.Request;
using PersonalLibrary.Models.Response;
using PersonalLibrary.Services;

namespace PersonalLibrary.Controllers;

[ApiController]
[Route("/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;
    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] QueryParameters parameters)
    {
        var allBooks = await _booksService.GetAllBooksResponse();
        var query = allBooks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.Trim();
            query = query.Where(b =>
            b.Id.Contains(searchTerm) ||
            b.Author.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
            (b.Translator ?? "").Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
            b.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
            b.Language.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
            (b.OriginalLanguage ?? "").Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
            (b.Notes ?? "").Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = SortQuery(parameters.SortBy, query);
        }
        else
        {
            query = query.OrderBy(b => b.SortTitle);
        }

        var skipAmount = (parameters.PageNumber - 1) * parameters.PageSize;
        query = query.Skip(skipAmount).Take(parameters.PageSize);

        return Ok(query.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] BookRequest newBook)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            await _booksService.Add(newBook);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return Ok();
    }

    [HttpPatch]
    [Route("{isbn}")]
    public async Task<IActionResult> UpdateReadStatus(string isbn)
    {
        if (isbn == null)
        {
            return BadRequest();
        }
        try
        {
            await _booksService.UpdateReadStatus(isbn);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return Ok();
    }

    public static IQueryable<BookResponse> SortQuery(string sortByTerm, IQueryable<BookResponse> query)
    {
        if (sortByTerm == "Author") return query.OrderBy(b => b.SortAuthor);
        else if (sortByTerm == "Translator") return query.OrderBy(b => b.SortTranslator == null).ThenBy(b => b.SortTranslator);
        else if (sortByTerm == "Publication") return query.OrderBy(b => b.PublicationYear);
        else return query.OrderBy(b => b.SortTitle);
    }
}