/*using BTLWeb.Data;*/
using Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Web.Data;

namespace Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private WebContext data;
        public ShoppingCartController(WebContext data)
        {
            this.data = data;
        }
        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";
        //lấy ra các phần tử của cart
        List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;// khai báo 1 session
            string jsoncart = session.GetString(CARTKEY);//lấy dữ liệu của cart
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);// list kiểu cart item ép kiểu là jsoncart
            }
            return new List<CartItem>();
        }
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }
        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            ViewBag.Total = updateTotal();
            return View("Index", GetCartItems());
        }
        public IActionResult AddToCart(int productId)
        {
            var product = data.TblProducts
                .Where(p => p.ProductId == productId)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productId);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
                cartitem.total = cartitem.quantity * cartitem.product.Price;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product ,total=product.Price});
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            return Ok();
            // return RedirectToAction(nameof(Cart));

        }
       
        public IActionResult UpdateCart(int productId, int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productId);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
                cartitem.total = cartitem.quantity * cartitem.product.Price;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            ViewBag.Total = updateTotal();
            return PartialView("CartItemProductId", cartitem);
        }
        /// xóa item trong cart
        /*[Route("/removecart/{productid:int}", Name = "removecart")]*/
        public IActionResult RemoveCart(int productId)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productId);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }
            SaveCartSession(cart);
            return PartialView("CartItem",cart);
        }
        public double updateTotal()
        {
            double total = 0;
            var cart = GetCartItems();
            foreach(var item in cart)
            {
                total += item.total;
            }
            return total;
        }
    }
}
