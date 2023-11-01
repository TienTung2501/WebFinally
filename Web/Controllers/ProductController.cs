using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(WebContext context)
        {
            _context = context;
        }
        //[Authorize(Roles = "admin")]
        public IActionResult Index(long? mid)
        {
            if(mid == null)
            {
                var listProducts = _context.TblProducts.Include(p => p.Category).ToList();
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
        public IActionResult Upsert(long? id)
        {
            ProductViewModel productVM = new()
            {
                CategoryList = _context.TblProducts.Select(u => new SelectListItem
                {
                    Text = u.NameProduct,
                    Value = u.ProductId.ToString()
                }),
                Product = new TblProduct()
            };
            if(id == null || id == 0) 
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _context.TblProducts.FirstOrDefault(u => u.ProductId == id) ;
                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, List<IFormFile> files)
        {
            if(ModelState.IsValid)
            {
                if(productVM.Product.ProductId == 0)
                {
                    _context.TblProducts.Add(productVM.Product);
                }
                else
                {
                    _context.TblProducts.Update(productVM.Product);
                }
                _context.SaveChanges();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\product\large-size\";
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        productVM.Product.Image = productPath + @"\" + fileName;
                    }
                    _context.TblProducts.Update(productVM.Product);
                    _context.SaveChanges();                   
                }
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _context.TblProducts.Select(u => new SelectListItem
                {
                    Text = u.NameProduct,
                    Value = u.ProductId.ToString()
                });
                return View(productVM);
            }
        }
        public IActionResult DeleteImage(int imageId)
        {
            var product = _context.TblProducts.FirstOrDefault(u => u.ProductId == imageId);
            if (product != null)
            {
                // Lưu đường dẫn ảnh cũ để xóa
                var oldImagePath = product.Image;

                // Nếu đường dẫn không rỗng, xóa hình ảnh từ đĩa
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Xóa thông tin hình ảnh
                product.Image = null;

                // Cập nhật thông tin sản phẩm trong cơ sở dữ liệu
                _context.TblProducts.Update(product);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Upsert), new { id = product?.ProductId });
        }

        [HttpGet]
        public IActionResult Delete(long? id) 
        {
            if(id == null || _context.TblProducts == null)
            {
                return NotFound();
            }
            var product = _context.TblProducts.Include(l => l.Category).FirstOrDefault(m => m.ProductId == id);
            
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
