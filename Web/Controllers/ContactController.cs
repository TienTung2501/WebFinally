using Microsoft.AspNetCore.Mvc;

namespace WebProgramming.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
