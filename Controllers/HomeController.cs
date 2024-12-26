using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLaptop.Models;
using MvcLaptop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
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
    public async Task<IActionResult> Index(string genre,string searchString)
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        ViewData["Genres"] = _context.Laptop
            .Select(l => l.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
        // Lọc sản phẩm theo Genre
        var laptops = string.IsNullOrEmpty(genre)
            ? _context.Laptop.AsQueryable() // Nếu không có genre, trả về tất cả sản phẩm
            : _context.Laptop.Where(l => l.Genre == genre); // Lọc theo genre
        if (!string.IsNullOrEmpty(searchString))
        {
            laptops = laptops.Where(l => l.Title!.ToUpper().Contains(searchString.ToUpper()));
        }
        // Ghi lại Genre hiện tại (nếu có)
        ViewData["CurrentGenre"] = genre;
        ViewData["SearchString"] = searchString;
        return View(await laptops.ToListAsync());
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
    public IActionResult Login(){
        return View();
    }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(string userName, string password)
{
    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
    {
        ModelState.AddModelError(string.Empty, "Username and password are required.");
        return View();
    }

    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

    if (user == null)
    {
        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View();
    }

    // Lưu thông tin người dùng vào session
    HttpContext.Session.SetString("UserName", user.UserName);

    return RedirectToAction(nameof(Index));
}

public IActionResult Logout()
{
    HttpContext.Session.Clear(); // Xóa thông tin phiên người dùng
    return RedirectToAction(nameof(Login));
}
    [HttpGet]
    public IActionResult Register()
        {
            return View();
        }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("UserName,Password,Email")] User user)
    {
        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }
        return View(user);
    }
}
