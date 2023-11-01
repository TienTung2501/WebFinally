using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.ViewModels;

namespace BTLWeb.Controllers
{
    public class ShopController : Controller
    {
        private WebContext db= new WebContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShopLeftSideBar(long? id)
        {
            
            var items =db.TblProducts.Take(12).ToList();
            if (id != null)
            {
                items=items.Where(p => p.ProductId == id).Take(12).ToList();
            }
            return View(items);
        }

        public IActionResult SingleProducts(long? id)
        {

            Console.WriteLine("HelloP: ");
            if (id == null)
            {
                id = 1; 
            }
            var currentItem = db.TblProducts.Find(id);

            var relatedProducts = db.TblProducts.Where(p => p.CategoryId == currentItem.CategoryId && p.ProductId != id).Take(15).ToList();
            // 
            var productList = new List<TblProduct>();
            productList.Add(currentItem); 
            productList.AddRange(relatedProducts); 

            // 
            return View(productList);
        }
        public IActionResult SearchFilterSort(string? searchString, string? categories, string? priceRanges, int? check, string? sortProduct, int? page,string? currentSearch, string? currentFilter)
        {
            // Khởi tạo danh sách dữ liệu gốc từ cơ sở dữ liệu.
            IQueryable<TblProduct> items = db.TblProducts;
            Console.WriteLine("Hello123");
            if (check == 1)
            {
                check = 0;
                page = 1;
                var vỉewModel2 = Pagination(items, page);
                return PartialView("_ListProductTest", vỉewModel2);
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentSearch;
            }
            if (categories != null)
            {

                page = 1;
            }
            else
            {
                categories = currentFilter;
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(p => p.NameProduct.Contains(searchString));
            }
           else if (!string.IsNullOrEmpty(categories))
            {
                List<string> selectedCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(categories);

                if (selectedCategories.Count != 0)
                {
                    items = items
                        .Join(db.TblCategories, p => p.CategoryId, c => c.CategoryId, (p, c) => new { p, CategoryName = c.CategoryName })
                        .Where(d => selectedCategories.Contains(d.CategoryName))
                        .Select(s => s.p);
                }
            }
            if (!string.IsNullOrEmpty(priceRanges))
            {
                string[] priceRangeParts = priceRanges.Split(" - ");
                float startPrice = float.Parse(priceRangeParts[0].Trim('$'));
                float endPrice = float.Parse(priceRangeParts[1].Trim('$'));
                items = items
                    .Where(s => s.Price >= startPrice && s.Price <= endPrice);
            }

            // Sắp xếp
            items =Sort_(items, sortProduct);
            // Phân trang
            var viewModel = Pagination(items, page);

            return PartialView("_ListProductTest", viewModel);
        }
        
        public IActionResult FilterSort(string? categories, string? priceRanges, int? check, string? sortProduct, int? page, string? currentFilter)
        {
            Console.WriteLine("Hi" + currentFilter);
            IQueryable<TblProduct> query = db.TblProducts;
            if (currentFilter== "NoCurrentFilter")
            {
                Console.WriteLine("HELO1234");
                currentFilter = "";
                var viewModel=Pagination(query, 1);
                return PartialView("_ListProductTest", viewModel);
            }
            if (check == 1)
            {
                check = 0;
                page = 1;
                var vỉewModel2 = Pagination(query, page);
                return PartialView("_ListProductTest", vỉewModel2);
            }

            //Sắp xếp
            query = Sort_(query, sortProduct);
            //Trả về phân trang
            return PartialView("_ListProductTest", Pagination(query, page));


        }
        private IQueryable<TblProduct> Sort_(IQueryable<TblProduct> items,string? sortProduct)
        {
            switch (sortProduct)
            {
                case "Relevance":
                    
                    break;
                case "sales":
                    items = items.OrderBy(p => p.NameProduct);
                    break;
                case "sales-desc":
                    items = items.OrderByDescending(p => p.NameProduct);
                    break;
                case "rating":
                    items = items.OrderBy(p => p.Price);
                    break;
                default:
                    // Mặc định hoặc trường hợp không hợp lệ.
                    break;
            }
            return items;

        }
        private ViewModelTest Pagination(IQueryable<TblProduct> items, int? page)
        {
            Console.WriteLine("PAGE:" + page+ " "+ items.Count());
            int pageSize = 5;
            int totalItems = items.Count();
            int totalPage = (int)Math.Ceiling((double)totalItems / pageSize);
            List<int> pageNumbers = Enumerable.Range(1, totalPage).ToList();
            int start = ((page ?? 1) - 1) * pageSize;
            Console.WriteLine("Start: " + start);
            var pagedItems = items.Skip(start).Take(pageSize).ToList();
            foreach(var x in pagedItems) { Console.WriteLine("Items " + x.NameProduct); };
            var viewModel = new ViewModelTest
            {
                DataList = pagedItems,
                PageNumbers = pageNumbers,
                CurrentPage = page ?? 1,
                TotalPages = totalPage
            };
            return viewModel;
        }
        public IActionResult Search(string? searchString)
        {

            IQueryable<TblProduct> items = db.TblProducts;
            if (!string.IsNullOrEmpty(searchString))
            {
                items=items.Where(s =>s.NameProduct.Contains(searchString));
            }
            return PartialView("_PhoneList", items.ToList());
        }
        public IActionResult Sort(string? sortProduct)
        {
            var items = db.TblProducts.ToList();
            switch (sortProduct)
            {
                case "trending":
                    break;
                case "sales":
                    // Sắp xếp theo tên từ A đến Z
                    items = items.OrderBy(p => p.NameProduct).ToList();
                    break;
                case "sales-desc":
                    // Sắp xếp theo tên từ Z đến A
                    items = items.OrderByDescending(p => p.NameProduct).ToList();
                    break;
                case "rating":
                    // Sắp xếp theo giá từ thấp đến cao
                    items = items.OrderByDescending(p => p.Price).ToList();
                    break;
                // Các trường hợp sắp xếp khác...

                default:
                    // Mặc định, hoặc trường hợp không hợp lệ
                    // Sắp xếp theo mặc định, ví dụ: Relevance
                    break;
            }

            return PartialView("_PhoneList",items);
        }
       
        public IActionResult FilterProducts(string? categories, string? priceRanges, int? check)
        {
            if (check == 1)
            {
                check = 0;
                var query = db.TblProducts.ToList();
                return PartialView("_PhoneList", query);
            }
            List<string> selectedCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(categories);

            if (selectedCategories.Count == 0 && string.IsNullOrEmpty(priceRanges))
            {
                var query = db.TblProducts.ToList();
                return PartialView("_PhoneList", query);
            }
            else
            {
                IQueryable<TblProduct> query = db.TblProducts;

                if (selectedCategories.Count != 0)
                {
                    query = query
                        .Join(db.TblCategories, p => p.CategoryId, c => c.CategoryId, (p, c) => new { p, CategoryName = c.CategoryName })
                        .Where(d => selectedCategories.Contains(d.CategoryName))
                        .Select(s => s.p);
                }

                Console.WriteLine("Tiền: " + priceRanges);

                if (!string.IsNullOrEmpty(priceRanges))
                {
                    string[] priceRangeParts = priceRanges.Split(" - ");
                    float startPrice = float.Parse(priceRangeParts[0].Trim('$'));
                    float endPrice = float.Parse(priceRangeParts[1].Trim('$'));
                    query = query
                        .Where(s => s.Price >= startPrice && s.Price <= endPrice);

                    Console.WriteLine("Tiền1: " + startPrice);
                    Console.WriteLine("Tiền2: " + endPrice);
                }
                foreach (var product in query.ToList())
                {
                    Console.WriteLine($"Product: {product.NameProduct}, Price: {product.Price}");
                }
                return PartialView("_PhoneList", query.ToList());
            }
        }
        public IActionResult PaginationAction(int page = 1)
        {
            int pageSize = 5;
            int totalItems = db.TblProducts.Count();
            int totalPage = (int)Math.Ceiling((double)totalItems / pageSize);
            List<int> pageNumbers = Enumerable.Range(1,totalPage).ToList();
            int start = (page - 1) * pageSize;  
            int end = start + pageSize;
            var query = db.TblProducts.Skip(start).Take(pageSize).ToList();
            var viewModel = new ViewModelTest
            {
                DataList = query,
                PageNumbers = pageNumbers,
                CurrentPage = page,
                TotalPages = totalPage
            };
            Console.WriteLine("PAGE: " + page);
            Console.WriteLine("Totals: " + totalPage);
            return PartialView("_ListProductTest", viewModel);
        }


    }
}
