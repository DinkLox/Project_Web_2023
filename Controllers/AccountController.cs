using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB_2023.Config;
using WEB_2023.Models.Auth;
using WEB_2023.Models.User;
using WEB_2023.Services;

namespace WEB_2023.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("SignUp")]
        public IActionResult SignUp(string returnUrl)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? "/");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new SignUpModel { });
        }
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin");
                return View(signUpModel);
            }
            var result = await _accountService.SignUpAsync(signUpModel);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(signUpModel);
            }
            TempData["SignUpStatus"] = "success";
            return RedirectToAction("SignUp");
        }
        [HttpGet]
        [Route("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail(string token, string uid)
        {
            // Check valid token and uid
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(uid))
            {
                return NotFound();
            }
            var result = await _accountService.ConfirmEmailAsync(token, uid);
            if (!result.IsSuccess)
            {
                return View();
            }
            TempData["ConfirmStatus"] = "success";
            return View();
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string returnUrl)
        {
            // Check if user is already logged in
            if (User?.Identity?.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? "/");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không chính xác");
                return View(loginModel);
            }
            var result = await _accountService.LoginAsync(loginModel);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(loginModel);
            }
            var userData = result.Data as UserViewModel;
            TempData["AuthMessage"] = "Đăng nhập thành công";
            TempData["Type"] = "success";
            if (userData?.Role != null && userData.Role == AppRoles.Admin && loginModel.ReturnUrl == "/")
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            if (!string.IsNullOrEmpty(loginModel.ReturnUrl) && Url.IsLocalUrl(loginModel.ReturnUrl))
            {
                if (userData?.Role != null && userData?.Role != AppRoles.Admin && loginModel.ReturnUrl.Contains("/admin"))
                    return RedirectToAction("Index", "Home");
                return Redirect(loginModel.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl = null!)
        {
            var LogoutResult = await _accountService.LogoutAsync();
            if (!LogoutResult.IsSuccess)
            {
                TempData["AuthMessage"] = LogoutResult?.Message ?? "Đăng xuất thất bại";
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Account", new { returnUrl });
        }
        [HttpGet]
        [Route("Forget-Password")]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("Forget-Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền email");
                return View(forgetPasswordModel);
            }
            var result = await _accountService.ForgetPasswordAsync(forgetPasswordModel);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(forgetPasswordModel);
            }
            TempData["status"] = "success";
            return RedirectToAction("ForgetPassword");
        }
        [HttpGet]
        [Route("Reset-Password")]
        public async Task<IActionResult> ResetPassword(string token, string uid)
        {
            // Check valid token and uid
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(uid))
            {
                return NotFound();
            }
            var result = await _accountService.CheckValidTokenAsync(token, uid);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return View(new ResetPasswordModel { Token = token, UserId = uid });
        }
        [HttpPost]
        [Route("Reset-Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin");
                return View(resetPasswordModel);
            }
            var result = await _accountService.ResetPasswordAsync(resetPasswordModel);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(resetPasswordModel);
            }
            TempData["AuthMessage"] = result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Login");
        }

    }
}