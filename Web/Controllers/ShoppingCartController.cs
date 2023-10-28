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
        /*[HttpPost("/ShoppingCart/AddToCart/{productId}")]
        [Route("addcart/{productid:int}", Name = "addcart")]*/
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
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));

        }
        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View("Index",GetCartItems());
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }
        /// xóa item trong cart
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }
    }
}
