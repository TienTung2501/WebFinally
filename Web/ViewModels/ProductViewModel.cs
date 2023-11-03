using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace Web.ViewModels
{
    public class ProductViewModel
    {
        [ValidateNever]
        public TblProduct Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
