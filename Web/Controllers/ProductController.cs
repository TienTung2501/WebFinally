using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.ViewModels;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebContext _context;

        public ProductController(WebContext context)
        {
            _context = context;
            
        }
        //[Authorize(Roles = "admin")]

        public IActionResult Index(long? mid)
        {
            if(mid == null)
            {
                 List<TblProduct> listProducts = _context.TblProducts.ToList();
                return View(listProducts);
            }
            else
            {
                var listProducts = _context.TblProducts.Where(l => l.CategoryId == mid).Include(c => c.Category).ToList();
                return View(listProducts);
            }               
        }
        public IActionResult ProductByCategory(int mid)
        {
            var listProducts = _context.TblProducts
                .Where(l => l.CategoryId == mid)
                .Include(c => c.Category).ToList();
            return PartialView("ProductTable", listProducts);
        }

        [HttpGet]
        public IActionResult Delete(long? id) 
        {
            if(id == null || _context.TblProducts == null)
            {
                return NotFound();
            }
            var product = _context.TblProducts.Include(l => l.Category).Include(o => o.TblOrderDetails).FirstOrDefault(m => m.ProductId == id);
            
            if(product == null) { return NotFound(); }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id) 
        {
            if(_context.TblProducts == null)
            {
                return Problem("product is null");
            }
            var product = _context.TblProducts.Find(id);
            if(product != null) 
            {
                _context.TblProducts.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Details(long? id)
        {
            if (id == null)
            {
                id = 1;
            }
            var currentItem = _context.TblProducts.Find(id);

            var relatedProducts = _context.TblProducts.Where(p => p.CategoryId == currentItem.CategoryId && p.ProductId != id).Take(15).ToList();
            // 
            var productList = new List<TblProduct>();
            productList.Add(currentItem);
            productList.AddRange(relatedProducts);

            // 
            return View(productList);
        }
    }
}
