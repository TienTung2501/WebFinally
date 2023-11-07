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
        private WebContext _context;

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

        //public IActionResult Create()
        //{
        //    var categories = new List<SelectListItem>();
        //    foreach(var item in _context.TblCategories)
        //    {
        //        categories.Add(new SelectListItem
        //        {
        //            Text = item.CategoryName,
        //            Value = item.CategoryId.ToString()
        //        });
        //    }
        //    ViewBag.CategoryId = categories;
        //    ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName");
        //    Console.WriteLine($"Exception:3");
        //    return View();
        //}
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public IActionResult Create([Bind("NameProduct, CategoryId, Decription, Price, Discount, Quantity, CreatedAt, UpdatedAt,PublishedAt, StartsAt, Image")] TblProduct product)
        //{
        //    try
        //    {
        //        _context = new WebContext();
        //        Console.WriteLine($"Exception:1");
        //        if (ModelState.IsValid)
        //        {
        //            Console.WriteLine($"Exception: 2 ");
        //            _context.TblProducts.Add(product);
        //            _context.SaveChanges();
        //            return RedirectToAction(nameof(Index));
        //        }
                
        //    }
        //    catch
        //    {

        //    }
        //    ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName");
        //    return View();
        //}

        //public IActionResult Edit(long? id)
        //{
        //    if (id == null || _context.TblProducts == null)
        //    {
        //        return NotFound();
        //    }

        //    var productEdit = _context.TblProducts.Find(id);

        //    if (productEdit == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName", productEdit.CategoryId);
        //    return View(productEdit);
        //}
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public IActionResult Edit(long? id, [Bind("ProductId,NameProduct, CategoryId, Decription, Price, Discount, Quantity, CreatedAt, UpdatedAt,PublishedAt, StartsAt, Image")] TblProduct product)
        //{
        //    if (id != product.ProductId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Cập nhật dữ liệu sản phẩm trong cơ sở dữ liệu dựa trên updatedProduct
        //            _context.TblProducts.Update(product);
        //            _context.SaveChanges();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.ProductId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index)); // Chuyển hướng đến trang Index hoặc trang chi tiết sản phẩm tùy thuộc vào yêu cầu của bạn.
        //    }
        //    ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName", product.CategoryId);
        //    return View(product);
        //}


        //private bool ProductExists(long? id)
        //{
        //    return (_context.TblProducts?.Any(e => e.ProductId == id)).GetValueOrDefault();
        //}

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
