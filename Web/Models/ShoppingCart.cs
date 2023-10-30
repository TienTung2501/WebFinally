using System.ComponentModel.DataAnnotations.Schema;
namespace Web.Models;
[NotMapped]
public class ShoppingCart
{
    // Đánh dấu thuộc tính này không tương ứng với cột trong cơ sở dữ liệu
    public List<CartItem> CartItems { get; set; }
    public double Total { get; set; }
}