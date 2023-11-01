using System;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public partial class TblCategory
    {
        public TblCategory()
        {
            TblProducts = new HashSet<TblProduct>();
        }

        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<TblProduct> TblProducts { get; set; }
    }
}
