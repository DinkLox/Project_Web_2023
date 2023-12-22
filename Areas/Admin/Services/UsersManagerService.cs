using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Utilities;
using WEB_2023.Models.User;

namespace WEB_2023.Areas.Admin.Services
{
    public interface IUsersManagerService
    {
        Task<ActionResponse> GetUsersListAsync(int? page, int? pageSize);
        // Task<ActionResponse> CreateUserAsync(CreateUserModel user);
        Task<ActionResponse> UpdateUserAsync(string id, EditUserModel user);
        Task<ActionResponse> GetUserByIdAsync(string userId);
        Task<ActionResponse> LockoutUserAsync(string userId);
        Task<ActionResponse> UnlockUserAsync(string userId);
        Task<ActionResponse> DeleteUserAsync(string userId);
        // Task<List<ImportUserModel>?> ImportMultiUserAsync(List<ImportUserModel>? users);
    }
    public class UsersManagerService : IUsersManagerService
    {
        private readonly ILogger<UsersManagerService> _logger;
        public readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersManagerService(UserManager<ApplicationUser> userManager, ApplicationDBContext context, IHttpContextAccessor httpContext, ILogger<UsersManagerService> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContext;
        }
        public async Task<ActionResponse> GetUsersListAsync(int? page, int? pageSize)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();
                var totalUser = await query.CountAsync();
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var totalPage = (int)Math.Ceiling((double)totalUser / currentPageSize);
                var result = await query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).ToListAsync();
                var users = result.Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    CreatedAt = user.CreatedAt,
                    IsLocked = _userManager.IsLockedOutAsync(user).Result
                }).ToList();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        totalUser,
                        totalPage,
                        currentPageSize,
                        currentPage,
                        users
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user list error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }

        public async Task<ActionResponse> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    throw new Exception("Người dùng không tồn tại");
                var UserInfo = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    CreatedAt = user.CreatedAt,
                    IsLocked = _userManager.IsLockedOutAsync(user).Result
                };
                return new ActionResponse
                {
                    IsSuccess = true,
                    Data = UserInfo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user by id error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return null!;
            }
        }
        public async Task<ActionResponse> UpdateUserAsync(string id, EditUserModel user)
        {
            try
            {
                var CurrentUser = await _userManager.FindByIdAsync(id);
                if (CurrentUser is null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Người dùng không tồn tại"
                    };
                }
                CurrentUser.Email = user.Email ?? CurrentUser.Email;
                CurrentUser.FullName = user.FullName ?? CurrentUser.FullName;
                CurrentUser.Address = user.Address ?? CurrentUser.Address;
                CurrentUser.PhoneNumber = user.Phone ?? CurrentUser.PhoneNumber;
                CurrentUser.DateOfBirth = user.DateOfBirth ?? CurrentUser.DateOfBirth;
                if (user.ImageFile != null)
                {
                    CurrentUser.Avatar = UploadImage.UploadSingleImage(user.ImageFile) ?? CurrentUser.Avatar;
                }
                var CurrentRole = await _userManager.GetRolesAsync(CurrentUser);
                if (!CurrentRole.Contains(user.RoleName) && user.RoleName != null)
                {
                    var RemoveRole = await _userManager.RemoveFromRolesAsync(CurrentUser, CurrentRole);
                    var AddRole = await _userManager.AddToRoleAsync(CurrentUser, user.RoleName);
                    if (!AddRole.Succeeded)
                    {
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Không thể cập nhật vai trò người dùng"
                        };
                    }
                }
                var Result = await _userManager.UpdateAsync(CurrentUser);
                if (!Result.Succeeded)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không thể cập nhật thông tin người dùng"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật thông tin người dùng thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        //* Create new user by admin
        // public async Task<ActionResponse> CreateUserAsync(CreateUserModel user)
        // {
        //     var IsEmailUsed = await _userManager.FindByEmailAsync(user.Email);
        //     if (IsEmailUsed != null)
        //     {
        //         return new ActionResponse
        //         {
        //             IsSuccess = false,
        //             Message = "Email đã được sử dụng"
        //         };
        //     }
        //     var NewUser = new ApplicationUser
        //     {
        //         Email = user.Email,
        //         UserName = user.Email,
        //         FirstName = user.FirstName,
        //         LastName = user.LastName,
        //         Address = user.GetAddress(),
        //         DateOfBirth = user.DateOfBirth,
        //         ProfileImage = UploadImage.UploadSingleImage(user.ProfileImage) ?? "/img/default-user.webp",
        //         CreatedAt = DateTime.Now,
        //         WorkspaceId = user.WorkspaceId,
        //     };
        //     var CreateNewUser = await _userManager.CreateAsync(NewUser, user.GeneratePassword());
        //     if (!CreateNewUser.Succeeded)
        //     {
        //         return new ActionResponse
        //         {
        //             IsSuccess = false,
        //             Message = "Không thể tạo người dùng, vui lòng thử lại sau!"
        //         };
        //     }
        //     var AddRole = await _userManager.AddToRoleAsync(NewUser, user.RoleName);
        //     if (!AddRole.Succeeded)
        //     {
        //         await _userManager.DeleteAsync(NewUser);
        //         return new ActionResponse
        //         {
        //             IsSuccess = false,
        //             Message = "Không thể tạo người dùng, lỗi phân quyền!"
        //         };
        //     }
        //     return new ActionResponse
        //     {
        //         IsSuccess = true,
        //         Message = "Tạo người dùng thành công!"
        //     };
        // }
        public async Task<ActionResponse> LockoutUserAsync(string userId)
        {
            if (userId == _userManager.GetUserId(_httpContextAccessor?.HttpContext?.User))
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể khóa tài khoản của chính mình"
                };
            }
            var user = await _userManager.FindByIdAsync(userId);
            var checkUserLock = await _userManager.IsLockedOutAsync(user);
            if (user == null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại"
                };
            }
            if (checkUserLock)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng này đã bị khóa từ trước"
                };
            }
            var setLocked = await _userManager.SetLockoutEnabledAsync(user, true);
            var lockUntil = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
            if (!setLocked.Succeeded || !lockUntil.Succeeded)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể khóa tài khoản người dùng"
                };
            }
            return new ActionResponse
            {
                IsSuccess = true,
                Message = "Khóa tài khoản người dùng thành công"
            };
        }
        public async Task<ActionResponse> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại"
                };
            }
            var checkUserLock = await _userManager.IsLockedOutAsync(user);
            if (!checkUserLock)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Tài khoản này không bị khóa"
                };
            }
            var lockUntil = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!lockUntil.Succeeded)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể mở khóa tài khoản này"
                };
            }
            return new ActionResponse
            {
                IsSuccess = true,
                Message = "Đã mở khóa tài khoản người dùng"
            };
        }
        public async Task<ActionResponse> DeleteUserAsync(string userId)
        {
            if (userId == _userManager.GetUserId(_httpContextAccessor?.HttpContext?.User))
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể xoá tài khoản của chính mình"
                };
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại"
                };
            }
            var Result = await _userManager.DeleteAsync(user);
            if (!Result.Succeeded)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Không thể xoá người dùng này"
                };
            }
            return new ActionResponse
            {
                IsSuccess = true,
                Message = "Đã xoá người dùng thành công"
            };
        }
        //* Import multi user by excel file
        // public async Task<List<ImportUserModel>?> ImportMultiUserAsync(List<ImportUserModel>? users)
        // {
        //     var Result = new List<ImportUserModel>();
        //     if (users?.Count < 0 || users == null)
        //         return null;
        //     foreach (var user in users)
        //         try
        //         {
        //             var IsEmailUsed = await _userManager.FindByEmailAsync(user.Email);
        //             if (IsEmailUsed != null)
        //             {
        //                 user.Status = "failed";
        //                 user.Message = "Email đã được sử dụng";
        //                 Result.Add(user);
        //                 continue;
        //             }
        //             var NewUser = new ApplicationUser
        //             {
        //                 UserName = user.Email,
        //                 Email = user.Email,
        //                 FirstName = user.FirstName,
        //                 LastName = user.LastName,
        //                 PhoneNumber = user.Phone,
        //                 DateOfBirth = user.DateOfBirth,
        //                 Address = user.GetAddress(),
        //                 CreatedAt = DateTime.Now,
        //                 ProfileImage = "/img/default-user.webp",
        //                 WorkspaceId = user.WorkspaceId
        //             };
        //             var ResultCreate = await _userManager.CreateAsync(NewUser, user.GeneratePassword());
        //             if (!ResultCreate.Succeeded)
        //             {
        //                 user.Status = "failed";
        //                 user.Message = "Thông tin không hợp lệ";
        //                 Result.Add(user);
        //                 continue;
        //             }
        //             var ResultAddRole = await _userManager.AddToRoleAsync(NewUser, user.RoleName);
        //             if (!ResultAddRole.Succeeded)
        //             {
        //                 await _userManager.DeleteAsync(NewUser);
        //                 user.Status = "failed";
        //                 user.Message = "Không thể phân quyền";
        //                 Result.Add(user);
        //                 continue;
        //             }
        //             user.Status = "succeeded";
        //             user.Message = "Đã thêm người dùng";
        //             Result.Add(user);
        //         }
        //         catch (Exception ex)
        //         {
        //             _logger.LogError(ex.Message);
        //             user.Status = "failed";
        //             user.Message = "Lỗi hệ thống";
        //             Result.Add(user);
        //         }
        //     return Result;
        // }
    }
}