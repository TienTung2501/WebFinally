using System;
using System.Collections.Generic;

namespace Web.Models
{
    public partial class TblOrder
    {
        public TblOrder()
        {
            TblBills = new HashSet<TblBill>();
            TblOrderDetails = new HashSet<TblOrderDetail>();
        }

        public long OrderId { get; set; }
        public string Token { get; set; } = null!;
        public string Status { get; set; } = null!;
        public double Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public virtual ICollection<TblBill> TblBills { get; set; }
        public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; }
    }
}
