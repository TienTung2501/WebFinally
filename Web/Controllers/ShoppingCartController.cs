/*using BTLWeb.Data;*/
using Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            ShowSession();
            return PartialView("CartIconUpdate");
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
        [Authorize (Roles="Admin,Customer")]
        public IActionResult CreateOrder()
        {
            double totalOder = 0;
            var user = HttpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Tạo một đơn đặt hàng mới
            var newOrder = new TblOrder
            {
                UserId = userId,
                Token = "", // Thêm token nếu cần
                Status = "Chưa Giao", // Thêm trạng thái đơn đặt hàng
                Total = 0, // Thêm tổng tiền ban đầu
                CreatedAt = DateTime.Now
            };

            // Thêm đơn đặt hàng vào cơ sở dữ liệu
            data.TblOrders.Add(newOrder);
            data.SaveChanges();

            var cart=GetCartItems();
            // Lặp qua danh sách sản phẩm trong giỏ hàng
            double totalOrder = 0;
            foreach (var item in cart)
            {
                // Tạo một chi tiết đơn đặt hàng mới
                var newOrderDetail = new TblOrderDetail
                {
                    ProductId = item.product.ProductId,
                    OrderId = newOrder.OrderId,
                    Price = item.product.Price,
                    Discount = 0, // Thêm giảm giá nếu cần
                    Quantity = item.quantity,
                    CreatedAt = DateTime.Now
                };

                // Thêm chi tiết đơn đặt hàng vào cơ sở dữ liệu
                data.TblOrderDetails.Add(newOrderDetail);

                // Cập nhật tổng tiền của đơn đặt hàng
                totalOrder += item.total;
            }

            // Cập nhật tổng tiền của đơn đặt hàng
            newOrder.Total = totalOrder;
            @ViewBag.Total=totalOrder;
            data.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            ViewBag.OrderId = newOrder.OrderId;
            ViewBag.OrderStatus = newOrder.Status;
            ViewBag.DateOrder = newOrder.CreatedAt.ToString("dd/MM/yyyy");
            return View("CreateOrder",cart);
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
        public void ShowSession()
        {
            var value = HttpContext.Session.GetString(CARTKEY);
            Console.WriteLine(value);
            // Trong đó "KeyName" là tên của session bạn muốn xem.
        }
    }
}
