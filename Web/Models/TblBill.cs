using System;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public partial class TblBill
    {
        public long BillId { get; set; }
        public long OrderId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public double Total { get; set; }

        public virtual TblOrder Order { get; set; } = null!;
    }
}
