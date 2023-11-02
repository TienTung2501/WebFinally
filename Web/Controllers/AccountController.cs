using Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;

namespace BTLWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;//là một đối tượng sử dụng để thực hiện các tác vụ liên quan đến quản lý người dùng, chẳng hạn như tạo, xóa, cập nhật người dùng,claim
        private readonly SignInManager<AppUser> _signInManager;//là đối tượng cho phép thực hiện đăng nhập và đăng xuất người dùng. Nó cung cấp các phương thức cho việc xác thực thông tin đăng nhập, tạo phiên làm việc, và nhiều tác vụ khác liên quan đến đăng nhập.
        private readonly RoleManager<IdentityRole> _roleManager;//là đối tượng được sử dụng để thực hiện các tác vụ quản lý vai trò (roles). Vai trò là một cách để tổ chức và kiểm soát quyền truy cập trong ứng dụng của bạn. Bạn có thể tạo, chỉnh sửa, xóa vai trò bằng RoleManager.
        //private readonly ISendGridEmail _sendGridEmail;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
            //ISendGridEmail sendGridEmail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            //_sendGridEmail = sendGridEmail;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackurl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                /*await _sendGridEmail.SendEmailAsync(model.Email, "Reset Email Confirmation", "Please reset email by going to this " +
                    "<a href=\"" + callbackurl + "\">link</a>");*/
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = "")
        {
            return code == "" ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "User not found");
                    return View();
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }
            return View(resetPasswordViewModel);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)// route muốn truy cập sau khi đăng nhập// lấy từ url của người dùng khi thực hiện phương thức get trên thanh url
            // trong trường hợp cụ thể thì khi một người đăng nhập vào thì sẽ được điều hướng đến 1 url cụ thể nào đó.
        {
            LoginViewModel loginViewModel = new()// tạo đối tượng login để lưu trữ được các dữ liệu gửi từ view lên
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")//gán thuộc tính route muốn đến của login bằng thằng truyền vào

                //được sử dụng để chuyển đổi đường dẫn tương đối sang đường dẫn tuyệt đối.
            };
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View(loginViewModel);
        }

        [HttpPost]// kết hợp thuộc tính post và yêu cầu csrf một cơ chế bảo mật
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName,
                                                                loginViewModel.Password,
                                                                loginViewModel.RememberMe,
                                                                lockoutOnFailure: true);// kiểm tra xác thực người dùng
                if (result.Succeeded)// nếu xác thực thành công rồi-> đăng nhập và có một số lựa chọn
                {
                    var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return Redirect("/user"); // Chuyển hướng đến trang /user nếu người dùng có vai trò "Admin"
                    }
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))// kiểm tra xem url truyền vào có rỗng không
                        // nếu không rỗng và bằng url hiện tại bằng url truyền vào thì cho về trang ban đầu
                    {
                        return Redirect(returnUrl); // Chuyển hướng đến trang ban đầu sau đăng nhập
                    }
                    else// nếu rỗng chuyển sang trang index
                    {
                        return RedirectToAction("Index", "Home"); // Hoặc chuyển hướng đến trang mặc định
                    }
                }

                if (result.IsLockedOut)// nếu người dùng đăng nhập quá nhiều  thì khóa
                {
                    return RedirectToAction("Index", "Home");
                }
                else // một vài lần
                {
                    ModelState.AddModelError(string.Empty, "Không đúng tên tài khoản hoặc mật khẩu");
                    return View(loginViewModel);
                }
            }
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            RegisterViewModel registerViewModel = new()
            {
                ReturnUrl = returnUrl
            };
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string? returnUrl = null)
        {
            registerViewModel.ReturnUrl = returnUrl;
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new AppUser { Email = registerViewModel.Email, UserName = registerViewModel.UserName };
                // TODO: Check if email already exists otherwise multiple users with the same email causes crash (Exception)
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                if (result.Errors.First().Code == "DuplicateUserName")
                {
                    ModelState.AddModelError("Username", $"Username {user.UserName} is already taken.");
                }
                else
                {
                    ModelState.AddModelError("Password", "User could not be created. Password not unique enough");
                }
            }

            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]// thực hiện hành động đăng xuất khi có yêu cầu post từ thằng có chứa mã thông báo
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Error", "Home");
        }
    }
}
