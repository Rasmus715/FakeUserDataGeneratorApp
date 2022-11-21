using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FakeUserDataGenerator.Models;
using FakeUserDataGenerator.Services;

namespace FakeUserDataGenerator.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public IActionResult UserData()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult _UserData(string seed, string country, double errors, int firstItem = 1)
    {
        if (country == "None")
            return StatusCode(204);
        var userData = _userService.GenerateUserData(seed, country, errors, firstItem);
       
        if (!userData.Any()) 
            return StatusCode(204); 
        
        //_userService.GenerateUserData(seed, "Russia", 0, firstItem);
          
        return PartialView(userData.ToList());
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}