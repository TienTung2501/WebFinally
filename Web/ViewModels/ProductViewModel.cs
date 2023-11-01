using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace Web.ViewModels
{
    public class ProductViewModel
    {
        public TblProduct Product { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
