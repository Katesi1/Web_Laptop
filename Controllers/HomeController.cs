using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;

namespace MvcLaptop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MvcLaptopContext _context;
    public HomeController(MvcLaptopContext context,ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        ViewData["Genres"] = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        var laptops = _context.Laptop.ToList();
        return View(laptops);
    }


    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Details(int id)
    {
        var laptop = _context.Laptop.FirstOrDefault(l => l.Id == id);
        if (laptop == null)
        {
            return NotFound();
        }
        return View(laptop);
    }
     // Action trả về danh sách Genre
    public IActionResult PartialGenres()
    {
        var genres = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        return PartialView("_GenreMenu", genres);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
