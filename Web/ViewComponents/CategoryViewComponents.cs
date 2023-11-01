using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.ViewModels;

namespace Web.ViewComponents
{
    public class  CategoryViewComponent:ViewComponent
    {
        private WebContext db;
        List<TblCategory> categorys;
        public CategoryViewComponent(WebContext _context)
        {
            db = _context;
            categorys = db.TblCategories.ToList();
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("RenderCategory", categorys);
        }
    }

}
