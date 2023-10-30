using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [NotMapped]// không thêm model này vào csdl khi migration
    public class CartItem
    {
        public int quantity { set; get; }
        public TblProduct product { set; get; }
        public double total { set; get; }
    }
}
