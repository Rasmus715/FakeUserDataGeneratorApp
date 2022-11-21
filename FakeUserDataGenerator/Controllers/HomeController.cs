using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FakeUserDataGenerator.Models;
using FakeUserDataGenerator.Services;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;

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
        
        return PartialView(userData.ToList());
    }

    [HttpGet]
    public ActionResult DownloadCsv()
    {
        var array = TempData.Get<byte[]>("Output");
        if (array != null)
        {
            return File(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UserList-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
        }

        return new EmptyResult();
    }

    [HttpPost]
    public ActionResult Export(IFormCollection formCollection)
    {
        //Console.WriteLine(formCollection.Keys.Count);
        /*foreach (var x in formCollection)
        {
            Console.WriteLine("Key: " + x.Key);
            Console.WriteLine("Value: " + x.Value);
        }*/
        
        //Console.WriteLine(formCollection.Count);

        var userList = new List<UserData>();
        for (var i = 0; i < formCollection.Keys.Count / 5 - 1; i++)
        {
            var user = new UserData
            {
                Index = formCollection[$"userList[{i}][Index]"],
                Id = formCollection[$"userList[{i}][Id]"],
                Name = formCollection[$"userList[{i}][Name]"],
                Address = formCollection[$"userList[{i}][Address]"],
                PhoneNumber = formCollection[$"userList[{i}][PhoneNumber]"]
            };
            userList.Add(user);
        }
        
        var stream = new MemoryStream();
        using var package = new ExcelPackage(stream);
        var workSheet = package.Workbook.Worksheets.Add("Sheet1");  
        workSheet.Cells.LoadFromCollection(userList, true);  
        package.Save();  
        return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserData.csv");
    }
    
    /*[HttpPost]
    public async Task<IActionResult> Export(List<UserData> userList)
    {
        Console.WriteLine(userList.Count);
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Worksheet 1");
        worksheet.Cells.LoadFromCollection(userList);
        //TempData.Put("Output", await package.GetAsByteArrayAsync());
        //return Json("Success");
        return File(await package.GetAsByteArrayAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UserList-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
    }*/
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}