using Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class ShopController : Controller
    {
        //private BTLWebContext _context = new BTLWebContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShopLeftSideBar()
        {
            return View();
        }
        public IActionResult SingleProducts()
        {
            return View();
        }
        //public IActionResult Detail(int id)
        //{
        //    // Lấy thông tin chi tiết đơn hàng dựa trên id được truyền vào
        //    var order = _context.TblProducts.FirstOrDefault(p => p.ProductId == id);

        //    if (order == null)
        //    {
        //        // Xử lý trường hợp không tìm thấy đơn hàng
        //        return NotFound();
        //    }

        //    return PartialView("SingleProducts"); // Trả về view hiển thị chi tiết đơn hàng
        //}
    }
}
