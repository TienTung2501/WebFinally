using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public partial class TblProduct
    {
        public TblProduct()
        {
            TblOrderDetails = new HashSet<TblOrderDetail>();
        }

        public long ProductId { get; set; }
        [Required]
        public string? NameProduct { get; set; } = null!;
        [Required]
        public long CategoryId { get; set; }
        [Required]
        public string? Decription { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Discount { get; set; }
        [Required]
        public short Quantity { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        [Required]
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public DateTime? PublishedAt { get; set; }
        [Required]
        public DateTime? StartsAt { get; set; }
        [Required]
        public string Image { get; set; }
        [ValidateNever]
        public virtual TblCategory? Category { get; set; } = null!;
        public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; }
    }
}
