using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Web.Data;
using Web.Models;

namespace Web.ViewComponents
{
    [ViewComponent(Name = "Category")] // Add the ViewComponentAttribute with the appropriate name
    public class TblCategoryViewComponent : ViewComponent // Rename the class to end with 'ViewComponent'
    {
        private readonly WebContext _context;
        public TblCategoryViewComponent(WebContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.TblCategories.ToListAsync(); // Assuming TblCategories is the DbSet for your categories
            return View("RenderCategory", categories);
        }
    }

}
