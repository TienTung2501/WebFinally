using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Microsoft.EntityFrameworkCore;
using Web.Data;

namespace WebProgramming.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly WebContext _context;
        public HomeController(WebContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult NewArrivals()
        {
            
            return View("_NewArrivals");
        }

        public IActionResult BestSeller()
        {          
            return PartialView("BestSeller");
        }

        public IActionResult PartialSamSung()
        {
            return PartialView("_PartialSamsungProducts");
        }
        public IActionResult PartialOppo()
        {
            return PartialView("_PartialOppoProducts");
        }
    }
}