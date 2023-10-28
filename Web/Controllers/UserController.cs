/*using BTLWeb.Data;*/
using Web.Data;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Web.ViewModels;

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

        [Authorize(Roles = "admin")]
        public IActionResult Index()//hiển thị người dùng và role để có thể chỉnh sửa
        {
            var query = from user in _db.Users
                        join userRole in _db.UserRoles on user.Id equals userRole.UserId
                        join role in _db.Roles on userRole.RoleId equals role.Id
                        select new
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            RoleId = role.Id,
                            RoleName = role.Name
                        };

            var results = query.ToList();


            return View(results);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Edit(string userId)// chỉnh sửa vai trò người dùng
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Redirect("/Index");

            }
            var query = from users in _db.Users // lọc ra các role của người dùng theo id
                        join userRoles in _db.UserRoles on user.Id equals userRoles.UserId
                        join rol in _db.Roles on userRoles.RoleId equals rol.Id 
                        where userId== user.Id
                        select new
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            RoleId = rol.Id,
                            RoleName = rol.Name
                        };

            var results = query.ToList();


           ViewBag.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user)// chỉnh sửa người dùng khi post lên
        {
            if (ModelState.IsValid)
            {
                var userDbValue = _db.Users.FirstOrDefault(x => x.Id == user.Id);// tìm người dùng theo id
                if (userDbValue == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(x => x.UserId == userDbValue.Id);// tìm role cho người dùng theo id
                if (userRole != null)// nếu tồn tại thì xóa bỏ
                {
                    var previousRoleName = _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefault();
                    await _userManager.RemoveFromRoleAsync(userDbValue, previousRoleName);
                }

                await _userManager.AddToRoleAsync(userDbValue, _db.Roles.FirstOrDefault(x => x.Id == user.Id).Name);// nếu chưa tồn tại thì thêm mới
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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
