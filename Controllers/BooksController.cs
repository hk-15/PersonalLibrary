using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalLibrary.Models;
using PersonalLibrary.Databases;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

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
            .OrderBy(b => b.Title)
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
        if (data["title"].ToString == null | data["author"].ToString == null)
        {
            TempData["Message"] = "Please enter a title and author before proceeding.";
            return RedirectToAction("Add"); 
        }
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
        _ = bool.TryParse(data["read"], out bool read);
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

                var newBook = new Book { Title = data["title"]!, Author = author!, Translator = data["translator"], OriginalLanguage = data["lang"], Collection = collection, PublicationYear = fYear, EditionPublicationYear = eYear, Isbn = data["isbn"], Notes = data["notes"], Read = read };

                _context.Books.Add(newBook);
                _context.SaveChanges();
                TempData["Message"] = "New book added";
                return RedirectToAction("Add");
            }
        }
        else
        {
            var newAuthor = new Author { Name = data["author"]! };
            _context.Authors.Add(newAuthor);

            var newBook = new Book { Title = data["title"]!, Author = newAuthor, Translator = data["translator"], OriginalLanguage = data["lang"], Collection = collection, PublicationYear = fYear, EditionPublicationYear = eYear, Isbn = data["isbn"], Notes = data["notes"], Read = read };

            _context.Books.Add(newBook);
            _context.SaveChanges();
            TempData["Message"] = "New book and author added";
            return RedirectToAction("Add");
        }
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var book = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Collection)
            .Single(b => b.BookId == id);
        if (book == null)
        {
            return NotFound();
        }
        var bookDto = new BookDto()
            {
                BookDtoId = book.BookId,
                Isbn = book.Isbn,
                Title = book.Title,
                AuthorName = book.Author!.Name,
                Translator = book.Translator,
                OriginalLanguage = book.OriginalLanguage,
                Collection = book.Collection!.CollectionName,
                PublicationYear = book.PublicationYear,
                EditionPublicationYear = book.EditionPublicationYear,
                Read = book.Read,
                Notes = book.Notes
            };

        return View(bookDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookDto bookDto)
    {
        if (id != bookDto.BookDtoId)
        {
            return NotFound();
        }
        
        var oldBook = _context.Books.AsNoTracking().Single(book => book.BookId == id);
  
        var authorExists = _context.Authors
            .Any(a => a.Name!
            .Equals(bookDto.AuthorName));
        var collectionExists = _context.Collections
            .Any(c => c.CollectionName
            .Equals(bookDto.Collection));

        if (!collectionExists)
        {
            TempData["Message"] = "Record not updated. Please check the Collection field for a typo.";
            return View();
        }
        if (ModelState.IsValid)
        {
            try
            {
                if (authorExists)
                {
                    var author = _context.Authors
                        .Single(a => a.Name == bookDto.AuthorName);
                    var collection = _context.Collections.Single(c => c.CollectionName == bookDto.Collection);

                    var newBook = new Book
                    {
                        BookId = id,
                        Isbn = bookDto.Isbn,
                        Title = bookDto.Title,
                        AuthorId = author.AuthorId,
                        Translator = bookDto.Translator,
                        OriginalLanguage = bookDto.OriginalLanguage,
                        CollectionId = collection.CollectionId,
                        PublicationYear = bookDto.PublicationYear,
                        EditionPublicationYear = bookDto.EditionPublicationYear,
                        Read = bookDto.Read,
                        Notes = bookDto.Notes
                    };
                    _context.Books.Update(newBook);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newAuthor = new Author
                    {
                        Name = bookDto.AuthorName,
                    };
                    _context.Authors.Add(newAuthor);
                    _context.SaveChanges();

                    var collection = _context.Collections.Single(c => c.CollectionName == bookDto.Collection);

                    var newBook = new Book
                    {
                        BookId = id,
                        Isbn = bookDto.Isbn,
                        Title = bookDto.Title,
                        AuthorId = newAuthor.AuthorId,
                        Translator = bookDto.Translator,
                        OriginalLanguage = bookDto.OriginalLanguage,
                        CollectionId = collection.CollectionId,
                        PublicationYear = bookDto.PublicationYear,
                        EditionPublicationYear = bookDto.EditionPublicationYear,
                        Read = bookDto.Read,
                        Notes = bookDto.Notes
                    };
                    _context.Books.Update(newBook);
                    await _context.SaveChangesAsync();

                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (oldBook != null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Catalogue");
        }
        return View(bookDto);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.BookId == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Catalogue");
    }

    public async Task<IActionResult> ReadStatus(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.BookId == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost, ActionName("ReadStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReadStatus(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            if (book.Read == false)
            {
                book.Read = true;
            }
            else
            {
                book.Read = false;
            }
            _context.Books.Update(book);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Catalogue");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}