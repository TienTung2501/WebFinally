/*using BTLWeb.Data;*/
using Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class ShopingCartController : Controller
    {
        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";
        //lấy ra các phần tử của cart
        List<TblOrderDetail> GetCartItems()
        {

            var session = HttpContext.Session;// khai báo 1 session
            string jsoncart = session.GetString(CARTKEY);//lấy dữ liệu của cart
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<TblOrderDetail>>(jsoncart);// list kiểu cart item ép kiểu là jsoncart
            }
            return new List<TblOrderDetail>();
        }
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }
        public IActionResult Index()
        {
           
           
            return View();
        }
    }
}
