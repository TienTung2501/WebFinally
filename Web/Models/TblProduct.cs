using System;
using System.Collections.Generic;

namespace Web.Models
{
    public partial class TblProduct
    {
        public TblProduct()
        {
            TblOrderDetails = new HashSet<TblOrderDetail>();
        }

        public long ProductId { get; set; }
        public string NameProduct { get; set; } = null!;
        public long CategoryId { get; set; }
        public string? Decription { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public short Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public string? Image { get; set; }

        public virtual TblCategory Category { get; set; } = null!;
        public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; }
    }
}
