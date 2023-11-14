using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Web.Data;
using Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private int pageSize = 5;
        private readonly WebContext _context ;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IWebHostEnvironment _hostingEnvironment;
        public ProductController(WebContext context, IWebHostEnvironment webHostEnvironment/*, IWebHostEnvironment hostingEnvironment*/)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            //_hostingEnvironment = hostingEnvironment;
        }
        //[Authorize(Roles = "admin")]

        public IActionResult Index(long? mid)
        {
            var listProducts = (IQueryable<TblProduct>)_context.TblProducts.Include(m => m.Category);
            if(mid != null)
            {
                listProducts = (IQueryable<TblProduct>)_context.TblProducts.Where(l => l.CategoryId == mid).Include(c => c.Category);               
            }   
            // tính số trang
            int pageNumber = (int)Math.Ceiling(listProducts.Count() / (float)pageSize);
            ViewBag.pageNumber = pageNumber;
            var productIndex = listProducts.Take(pageSize).ToList();
            return View(productIndex);
        }
        public IActionResult ProductFilter( string ? keyword, int? pageIndex)
        {
            var products = (IQueryable<TblProduct>)_context.TblProducts;
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            if(keyword != null)
            {
                products = products.Where(l => l.NameProduct.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;  
            }
            int pageNumber = (int)Math.Ceiling(products.Count() / (float)pageSize);
            ViewBag.pageNumber = pageNumber;

            var listNewProduct = products.Skip(pageSize * (page-1)).Take(pageSize).Include(c => c.Category).ToList();

            return PartialView("ProductTable",listNewProduct);
        }
        //public IActionResult ProductByCategory(int mid)
        //{
        //    var listProducts = _context.TblProducts
        //        .Where(l => l.CategoryId == mid)
        //        .Include(c => c.Category).ToList();
        //    return PartialView("ProductTable", listProducts);
        //}

        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.TblCategories , "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("NameProduct ,CategoryId, Decription, Price, Discount, Quantity, CreatedAt,UpdatedAt,PublishedAt,StartsAt,Image ")] TblProduct product)
        {
            if(ModelState.IsValid)
            {
                _context.TblProducts.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpGet]
        public IActionResult Edit(long? id) 
        { 
            if(id == null || _context.TblProducts == null)
            {
                return NotFound();
            }
            var product = _context.TblProducts.Find(id);
            if(product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName",product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, [Bind("ProductId,NameProduct,CategoryId,Decription,Price,Discount,Quantity,CreatedAt,UpdatedAt,PublishedAt,StartsAt,Image")] TblProduct product, IFormFile files)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra nếu có file được chọn
                    if (files != null && files.Length > 0)
                    {
                        // Lấy đường dẫn đến thư mục wwwroot\images\product\large-size
                        var largeSizePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "product", "large-size");

                        // Kiểm tra nếu ảnh chưa tồn tại, thì sao chép vào
                        var imagePath = Path.Combine(largeSizePath, files.FileName);
                        if (!System.IO.File.Exists(imagePath))
                        {
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                files.CopyTo(stream);
                            }
                        }

                        // Cập nhật đường dẫn ảnh trong model
                        product.Image = files.FileName;
                    }

                    // Tiếp tục cập nhật thông tin sản phẩm và lưu vào cơ sở dữ liệu
                    _context.TblProducts.Update(product);
                    _context.SaveChanges();
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.CategoryId = new SelectList(_context.TblCategories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        private bool ProductExists (long ?id)
        {
            return (_context.TblProducts?.Any(e => e.ProductId == id)).GetValueOrDefault();
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
