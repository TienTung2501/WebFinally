using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Data;

namespace Web.Controllers
{
    [Authorize (Roles ="Admin,Customer")]
    public class OrderController : Controller
    {
        
        private  string UserID;
        private WebContext data;
        public OrderController(WebContext webContext) {
            data = webContext;
        }
        public IActionResult Index()
        {
            var user = HttpContext.User;
            UserID = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orderlist=data.TblOrders.Include(m=>m.User).Where(m=>m.UserId==UserID).ToList();
            ViewBag.Username = user.FindFirst(ClaimTypes.Name).Value;
            return View(orderlist);
        }
        public IActionResult OrderDetail(int id,string status,DateTime date) {
            ViewBag.ID = id;
            ViewBag.Status = status;
            ViewBag.Date = date.ToString("dd/MM/yyyy");
            double total = 0;
            var orderDetail=data.TblOrderDetails.Include(n=>n.Product).Where(m=>m.OrderId==id).ToList();
            foreach(var order in orderDetail)
            {
                total += order.Quantity * order.Price;
            }
            ViewBag.Total = total;
            return View(orderDetail);
        }
    }
}
