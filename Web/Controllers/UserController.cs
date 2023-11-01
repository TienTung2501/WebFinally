/*using BTLWeb.Data;*/
using Web.Data;
using Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data;
// username:tung123/Tung.123
namespace Web.Controllers
{
    public class UserController : Controller
    {
        private readonly WebContext _db;
        private readonly UserManager<AppUser> _userManager;

        public UserController(WebContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()//hiển thị người dùng và role để có thể chỉnh sửa
        {
            var query = from user in _db.Users
                        join userRole in _db.UserRoles on user.Id equals userRole.UserId
                        join role in _db.Roles on userRole.RoleId equals role.Id
                        select new UserRoleViewModel //  khai báo kiểu dữ liệu cho viewModels
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            UserEmail = user.Email,
                            RoleId = role.Id,
                            RoleName = role.Name
                        };

            var results = query.ToList();


            return View(results);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string userId)// chỉnh sửa vai trò người dùng
        {
            UserRoleViewModel userRol = new UserRoleViewModel();
            /* var user = _db.Users.FirstOrDefault(u => u.Id == userId);
             var role = _db.Roles.Include(n=>n.).FirstOrDefault(m=>m.Id == userId);*/

            var userAndRole = (from user in _db.Users
                               join userRole in _db.UserRoles on user.Id equals userRole.UserId
                               join role in _db.Roles on userRole.RoleId equals role.Id
                               where user.Id == userId
                               select new UserRoleViewModel()
                               {
                                   UserId = user.Id,
                                   UserName = user.UserName,
                                   UserEmail = user.Email,
                                   RoleId = role.Id,
                                   RoleName = role.Name,
                                   // Bổ sung các thuộc tính khác từ các bảng khác tại đây
                               }).FirstOrDefault();

            if (userAndRole != null)
            {
         
                userRol.UserId = userAndRole.UserId;
                userRol.UserName= userAndRole.UserName;
                userRol.UserEmail = userAndRole.UserEmail;
                userRol.RoleId = userAndRole.RoleId;
                userRol.RoleName = userAndRole.RoleName;
            }
            else
            {
                // Xử lý khi không tìm thấy dữ liệu
            }
           // Trong ví dụ này, chúng tôi sử dụng Include để nạp dữ liệu liên quan từ bảng Users và Roles.Sau đó, chúng tôi sử dụng Select để chọn các thuộc tính mà bạn muốn và tạo một đối tượng mới.
           ViewBag.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(userRol);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoleViewModel user)// chỉnh sửa người dùng khi post lên
        {
            if (ModelState.IsValid)
            {
                var userDb = _db.Users.FirstOrDefault(x => x.Id == user.UserId);// tìm người dùng theo id
                if (userDb == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(x => x.UserId == userDb.Id);// tìm role cho người dùng theo id
                if (userRole != null)// nếu tồn tại thì xóa bỏ
                {
                    var previousRoleName = _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefault();
                    await _userManager.RemoveFromRoleAsync(userDb, previousRoleName);
                }

                await _userManager.AddToRoleAsync(userDb, _db.Roles.FirstOrDefault(x => x.Id == user.RoleId).Name);// nếu chưa tồn tại thì thêm mới
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RolesList = _db.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(string userId)// xóa người dùng
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageClaims(string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            IList<Claim> existingUserClaims = await _userManager.GetClaimsAsync(user);

            UserClaimsViewModel model = new()
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimStore.claimsList)
            {
                UserClaim userClaim = new()
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageClaims(UserClaimsViewModel userClaimsViewModel)
        {
            var user = await _userManager.FindByIdAsync(userClaimsViewModel.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                return View(userClaimsViewModel);
            }

            result = await _userManager.AddClaimsAsync(user,
                userClaimsViewModel.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
                );

            if (!result.Succeeded)
            {
                return View(userClaimsViewModel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
