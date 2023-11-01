using Microsoft.AspNetCore.Mvc;
using Web.Data;

namespace BTLWeb.ViewComponents
{
    public class PriceViewComponent : ViewComponent
    {
        private WebContext db;
        double _MaxPrice;
        public PriceViewComponent(WebContext _context)
        {
            db = _context;
            _MaxPrice = db.TblProducts.Max(p =>p.Price);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("RenderPriceRange", _MaxPrice);
        }


    }
}