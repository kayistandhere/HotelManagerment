using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hotel.Models;
using Hotel.Base;
using Hotel.Data;

namespace Hotel.Controllers;

public class HomeController : BaseController
{
    public HomeController(QlksdbContext context) : base(context)
    {
    }

    public IActionResult Index()
    {
        return base.ChuyenHuong();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
