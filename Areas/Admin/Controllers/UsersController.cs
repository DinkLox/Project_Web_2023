using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Areas.Admin.Services;
using WEB_2023.Config;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Models.User;

namespace WEB_2023.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : Controller
    {
        public readonly IUsersManagerService _usersManagerService;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _context;
        public UsersController(ILogger<UsersController> logger, UserManager<ApplicationUser> userManager, ApplicationDBContext context, IUsersManagerService usersManagerService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _usersManagerService = usersManagerService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _usersManagerService.GetUsersListAsync(page, pageSize);
            if (response.IsSuccess)
            {
                var data = response.Data as dynamic;
                ViewBag.TotalUser = data?.totalUser;
                ViewBag.TotalPage = data?.totalPage;
                ViewBag.currentPageSize = data?.currentPageSize;
                ViewBag.CurrentPage = data?.currentPage;
                var users = data?.users as List<UserViewModel>;
                return View(users);
            }
            return NotFound();
        }
        // [HttpGet]
        // [Route("Create")]
        // public IActionResult CreateUser()
        // {
        //     return View();
        // }

        // [HttpPost]
        // [Route("Create")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> CreateUser(CreateUserModel model)
        // {
        //     if (ModelState.IsNullOrEmpty())
        //     {
        //         return View();
        //     }
        //     var Result = await _usersManagerService.CreateUserAsync(model);
        //     if (!Result.IsSuccess)
        //     {
        //         ModelState.AddModelError(string.Empty, Result.Message);
        //         return View(model);
        //     }
        //     TempData["UsersMessage"] = "Thêm người dùng thành công";
        //     TempData["Type"] = "success";
        //     return RedirectToAction("Index", "Users");
        // }
        [HttpGet]
        [Route("{id}/View")]
        public async Task<IActionResult> ViewUser(string id)
        {
            var result = await _usersManagerService.GetUserByIdAsync(id);
            if (result.IsSuccess)
            {
                var user = result.Data as UserViewModel;
                return View(user);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("{id}/Edit")]
        public async Task<IActionResult> EditUser(string id)
        {
            var result = await _usersManagerService.GetUserByIdAsync(id);
            if (result.IsSuccess)
            {
                var UserInfo = result.Data as UserViewModel;
                return View((UserInfo, new EditUserModel()));
            }
            TempData["UsersMessage"] = result.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        [Route("{id}/Edit")]
        public async Task<IActionResult> UpdateUser([Bind(Prefix = "Item2")] EditUserModel userModel, string id)
        {
            if (ModelState.IsNullOrEmpty())
            {
                TempData["UsersMessage"] = "Không có dữ liệu nào được gửi";
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Users");
            }
            var Result = await _usersManagerService.UpdateUserAsync(id, userModel);
            if (!Result.IsSuccess)
            {
                TempData["UsersMessage"] = Result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Users");
            }
            TempData["UsersMessage"] = Result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("ViewUser", "Users", new { id = id });
        }
        [HttpPost]
        [Route("Lockout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lockout(string id)
        {
            var result = await _usersManagerService.LockoutUserAsync(id);
            if (!result.IsSuccess)
            {
                TempData["UsersMessage"] = result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Users");
            }
            TempData["UsersMessage"] = result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        [Route("Unlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var result = await _usersManagerService.UnlockUserAsync(id);
            if (!result.IsSuccess)
            {
                TempData["UsersMessage"] = result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Users");
            }
            TempData["UsersMessage"] = result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var Result = await _usersManagerService.DeleteUserAsync(id);
            if (!Result.IsSuccess)
            {
                TempData["UsersMessage"] = Result.Message;
                TempData["Type"] = "error";
                return RedirectToAction("Index", "Users");
            }
            TempData["UsersMessage"] = Result.Message;
            TempData["Type"] = "success";
            return RedirectToAction("Index", "Users");
        }
        // [HttpGet]
        // [Route("Import")]
        // public IActionResult Import(string type)
        // {
        //     switch (type)
        //     {
        //         case "excel":
        //             return View("ImportExcel");
        //         case "url":
        //             return NotFound();
        //         default:
        //             return NotFound();
        //     }
        // }
        // [HttpPost]
        // [Route("Import/Excel")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ImportExcel(string UserList)
        // {
        //     var ValidUsers = JsonConvert.DeserializeObject<List<ImportUserModel>>(UserList);
        //     var ImportId = Guid.NewGuid().ToString();
        //     var Result = await _usersManagerService.ImportMultiUserAsync(ValidUsers);
        //     if (Result.IsNullOrEmpty())
        //     {
        //         TempData["UsersMessage"] = "Không có người dùng nào được thêm";
        //         TempData["Type"] = "warning";
        //         return RedirectToAction("Index", "Users");
        //     }
        //     TempData["ImportId"] = ImportId;
        //     TempData["Result"] = JsonConvert.SerializeObject(Result).ToString();
        //     return RedirectToAction("ImportResult", new { importId = ImportId });
        // }
        // [HttpGet]
        // [Route("Import/Result")]
        // public async Task<IActionResult> ImportResult(string? importId)
        // {
        //     var StoredImportId = TempData["ImportId"]?.ToString();
        //     var JSONData = TempData["Result"]?.ToString();
        //     if (StoredImportId != importId || StoredImportId == null || importId == null)
        //         return NotFound();
        //     var ImportedUsers = JsonConvert.DeserializeObject<List<ImportUserModel>>(JSONData ?? string.Empty);
        //     if (ImportedUsers.IsNullOrEmpty() || ImportedUsers?.Count == 0)
        //         return RedirectToAction("Index", "Users");
        //     var countSucceeded = ImportedUsers?.Count(user => user.Status == "succeeded");
        //     if (countSucceeded == 0)
        //     {
        //         TempData["UsersMessage"] = "Không có người dùng nào được thêm";
        //         TempData["Type"] = "warning";
        //         return RedirectToAction("Index", "Users");
        //     }
        //     TempData["UsersMessage"] = $"Đã thêm {countSucceeded} người dùng thành công";
        //     TempData["Type"] = "success";
        //     return View(ImportedUsers);
        // }
        // [HttpGet]
        // [Route("Import/Download")]
        // public IActionResult DownloadExcelTemplate()
        // {
        //     var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents", "DULIEUMAU.xlsx");
        //     if (!System.IO.File.Exists(path))
        //     {
        //         return NotFound();
        //     }
        //     var file = DownloadFiles.DownloadSingleFile(path);
        //     if (file == null)
        //     {
        //         return NotFound();
        //     }
        //     return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DULIEUMAU.xlsx");
        // }

    }
}