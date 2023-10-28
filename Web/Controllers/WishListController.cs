using Microsoft.AspNetCore.Mvc;

namespace WebProgramming.Controllers
{
    public class WishListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
