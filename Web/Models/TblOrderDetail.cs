using System;
using System.Collections.Generic;

namespace Web.Models
{
    public partial class TblOrderDetail
    {
        public long OrderDetailId { get; set; }
        public long ProductId { get; set; }
        public long OrderId { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual TblOrder Order { get; set; } = null!;
        public virtual TblProduct Product { get; set; } = null!;
    }
}
