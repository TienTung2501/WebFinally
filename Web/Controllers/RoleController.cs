﻿using Web.Data;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace BTLWeb.Controllers
{
    public class RoleController : Controller
    {
        private readonly WebContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(WebContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                var user = _db.Roles.FirstOrDefault(x => x.Id == id);
                return View(user);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRole role)
        {
            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(role.Id))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = role.Name });
            }
            else
            {
                var roleDb = _db.Roles.FirstOrDefault(x => x.Id == role.Id);
                if (roleDb == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                roleDb.Name = role.Name;
                roleDb.NormalizedName = role.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(roleDb);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var roleDb = _db.Roles.FirstOrDefault(u => u.Id == id);
            if (roleDb == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var userRolesForThisRole = _db.UserRoles.Where(u => u.RoleId == id).Count();
            if (userRolesForThisRole > 1)
            {
                return RedirectToAction(nameof(Index));
            }
            await _roleManager.DeleteAsync(roleDb);
            return RedirectToAction(nameof(Index));

        }
    }
}
