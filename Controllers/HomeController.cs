using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalLibrary.Models;
using PersonalLibrary.Databases;

namespace PersonalLibrary.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly PersonalLibraryContext _context;

    public HomeController(PersonalLibraryContext context, ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
