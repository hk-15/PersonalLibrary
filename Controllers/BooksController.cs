using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalLibrary.Models;
using PersonalLibrary.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PersonalLibrary.Controllers;

public class BooksController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly PersonalLibraryContext _context;

    public BooksController(PersonalLibraryContext context, ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
    }

    async public Task<IActionResult> Catalogue()
    {
        ViewBag.books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Collection)
            .ToListAsync();
        return View();
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    async public Task<IActionResult> Add(IFormCollection data)
    {

        var authorList = await _context.Authors.ToListAsync();
        var bookList = await _context.Books.ToListAsync();

        var authorExists = authorList
            .Any(a => a.Name!
            .Equals(data["author"], StringComparison.OrdinalIgnoreCase));
        var bookExists = bookList
            .Any(b => b.Title!
            .Equals(data["title"], StringComparison.OrdinalIgnoreCase));

        _ = int.TryParse(data["fyear"], out int fYear);
        _ = int.TryParse(data["eyear"], out int eYear);
        _ = Boolean.TryParse(data["read"], out bool read);
        string? collectionInput = data["collection"];

        var collection = _context.Collections.Single(c => c.CollectionName == collectionInput);
        if (authorExists)
        {
            if (bookExists)
            {
                TempData["Message"] = "This book already exists in the catalogue.";
                return RedirectToAction("Add");
            }
            else
            {
                var author = authorList.Find(author => author.Name == data["author"]);

                var newBook = new Book { Title = data["title"], Author = author, Translator = data["translator"], OriginalLanguage = data["lang"], Collection = collection, PublicationYear = fYear, EditionPublicationYear = eYear, Isbn = data["isbn"], Notes = data["notes"], Read = read };

                _context.Books.Add(newBook);
                _context.SaveChanges();
                TempData["Message"] = "New book added";
                return RedirectToAction("Add");
            }
        }
        else
        {
            var newAuthor = new Author { Name = data["author"] };
            _context.Authors.Add(newAuthor);

            var newBook = new Book { Title = data["title"], Author = newAuthor, Translator = data["translator"], OriginalLanguage = data["lang"], Collection = collection, PublicationYear = fYear, EditionPublicationYear = eYear, Isbn = data["isbn"], Notes = data["notes"], Read = read };

            _context.Books.Add(newBook);
            _context.SaveChanges();
            TempData["Message"] = "New book and author added";
            return RedirectToAction("Add");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}