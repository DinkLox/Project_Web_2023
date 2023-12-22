using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using WEB_2023.Config;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Models.Auth;
using WEB_2023.Models.User;
using WEB_2023.Utilities;

namespace WEB_2023.Services
{
    public interface IAccountService
    {
        Task<ActionResponse> SignUpAsync(SignUpModel signUpModel);
        Task<ActionResponse> LoginAsync(LoginModel loginModel);
        Task<ActionResponse> LogoutAsync();
        Task<ActionResponse> ForgetPasswordAsync(ForgetPasswordModel forgetPasswordModel);
        Task<ActionResponse> CheckValidTokenAsync(string token, string userId);
        Task<ActionResponse> ConfirmEmailAsync(string token, string userId);
        Task<ActionResponse> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
    }
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDBContext context, IHttpContextAccessor httpContextAccessor, ILogger<AccountService> logger, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        public async Task<ActionResponse> SignUpAsync(SignUpModel signUpModel)
        {
            try
            {
                if (signUpModel.Password != signUpModel.ConfirmPassword)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu nhập lại không khớp"
                    };
                }
                // Check user exists in database
                var userExists = await _userManager.FindByEmailAsync(signUpModel.Email) ?? await _userManager.FindByNameAsync(signUpModel.Email);
                if (userExists != null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng"
                    };
                }
                var user = new ApplicationUser
                {
                    UserName = signUpModel.Email,
                    Email = signUpModel.Email,
                    FullName = signUpModel.FullName,
                    Avatar = "/img/default-user.webp",
                    CreatedAt = DateTime.UtcNow,
                };
                // Check password is valid with password policy
                foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
                {
                    IdentityResult checkPassword;
                    checkPassword = await passwordValidator.ValidateAsync(_userManager, user, signUpModel.Password);
                    if (!checkPassword.Succeeded)
                    {
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt"
                        };
                    }
                }
                // Create user with password
                var result = await _userManager.CreateAsync(user, signUpModel.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.FirstOrDefault()?.Description);
                }
                // Add user to role customer
                await _userManager.AddToRoleAsync(user, AppRoles.Customer);
                // Generate token to confirm email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var endCodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                // Generate url to confirm email
                var callbackUrl = _httpContextAccessor?.HttpContext?.Request.Scheme + "://" + _httpContextAccessor?.HttpContext?.Request.Host + "/account/confirm-email?token=" + endCodeToken + "&uid=" + user.Id;
                var mailGunVariables = JsonConvert.SerializeObject(new
                {
                    callbackUrl,
                    name = user.FullName
                });
                // Send email to confirm email
                var sendEmailResult = await _emailService.SendEmailAsync(user.Email, "XÁC NHẬN ĐĂNG KÝ TÀI KHOẢN", EmailTemplate.ConfirmEmail, mailGunVariables);
                if (sendEmailResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await _userManager.DeleteAsync(user);
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Gửi email xác thực thất bại"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignUp error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<ActionResponse> LoginAsync(LoginModel loginModel)
        {
            try
            {
                // Check user exists in database
                var user = await _userManager.FindByEmailAsync(loginModel.Email) ?? await _userManager.FindByNameAsync(loginModel.Email);
                if (user == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Thông tin đăng nhập không chính xác"
                    };
                }
                // Check mail confirmed
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!isEmailConfirmed)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản này chưa được xác nhận email"
                    };
                }
                var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    // If login failed, increase access failed count
                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    int accessFailedRemaining = 5 - accessFailedCount;
                    // If user is locked out
                    if (result.IsLockedOut)
                    {
                        var getLockedTimeUntil = await _userManager.GetLockoutEndDateAsync(user);
                        var getLockedTime = getLockedTimeUntil - DateTime.Now;
                        // If user locked out less than 5 minutes, it means user has been temporarily locked out
                        if (getLockedTime.Value.TotalMinutes < 6)
                        {
                            return new ActionResponse
                            {
                                IsSuccess = false,
                                Message = $"Tài khoản bị khóa do đăng nhập sai nhiều lần, vui lòng thử lại sau: {getLockedTime.Value.ToString("mm")} phút"
                            };
                        }
                        // If user locked out more than 5 minutes, it means user has been permanently locked out
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Tài khoản của bạn đã bị khóa bởi quản trị viên"
                        };
                    }
                    if (accessFailedCount < 2)
                    {
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Thông tin đăng nhập không chính xác"
                        };
                    }
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = $"Thông tin đăng nhập không chính xác, bạn còn {accessFailedRemaining} lần thử"
                    };
                }
                // If login success, reset access failed count
                await _userManager.ResetAccessFailedCountAsync(user);
                // Get user role
                var roles = await _userManager.GetRolesAsync(user);
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    Data = new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        Avatar = user.Avatar,
                        Role = roles.FirstOrDefault(),
                        IsLocked = result.IsLockedOut,
                        DateOfBirth = user.DateOfBirth,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<ActionResponse> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Đăng xuất thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<ActionResponse> ForgetPasswordAsync(ForgetPasswordModel forgetPasswordModel)
        {
            try
            {
                // Check user exists in database
                var user = await _userManager.FindByEmailAsync(forgetPasswordModel.Email) ?? await _userManager.FindByNameAsync(forgetPasswordModel.Email);
                if (user == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Email không tồn tại"
                    };
                }
                // Generate token to reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var endCodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                // Generate url to reset password
                var callbackUrl = _httpContextAccessor?.HttpContext?.Request.Scheme + "://" + _httpContextAccessor?.HttpContext?.Request.Host + "/account/reset-password?token=" + endCodeToken + "&uid=" + user.Id;
                var mailGunVariables = JsonConvert.SerializeObject(new
                {
                    callbackUrl
                });
                // Send email to reset password
                var sendEmailResult = await _emailService.SendEmailAsync(user.Email, "XÁC NHẬN ĐẶT LẠI MẬT KHẨU", EmailTemplate.ForgetPassword, mailGunVariables);
                if (sendEmailResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Gửi email thất bại"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Vui lòng kiểm tra email để đặt lại mật khẩu"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgetPassword error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        // Check token is valid
        public async Task<ActionResponse> CheckValidTokenAsync(string token, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản không tồn tại"
                    };
                }
                var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", decodeToken);
                if (!isValid)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Token không hợp lệ"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Token hợp lệ"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckValidToken error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<ActionResponse> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                // Check user exists in database
                var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);
                if (user == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản không tồn tại"
                    };
                }
                // Check password and confirm password is match
                if (resetPasswordModel.Password != resetPasswordModel.ConfirmPassword)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu nhập lại không khớp"
                    };
                }
                // Check password is valid with password policy
                foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
                {
                    IdentityResult checkPassword;
                    checkPassword = await passwordValidator.ValidateAsync(_userManager, user, resetPasswordModel.Password);
                    if (!checkPassword.Succeeded)
                    {
                        return new ActionResponse
                        {
                            IsSuccess = false,
                            Message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt"
                        };
                    }
                }
                // Reset password
                var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordModel.Token));
                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", decodeToken);
                var result = await _userManager.ResetPasswordAsync(user, decodeToken, resetPasswordModel.Password);
                if (!result.Succeeded)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Đặt lại mật khẩu thất bại"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Đặt lại mật khẩu thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResetPassword error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }
        public async Task<ActionResponse> ConfirmEmailAsync(string token, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản không tồn tại"
                    };
                }
                var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, decodeToken);
                if (!result.Succeeded)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Xác nhận email thất bại"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Xác nhận email thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmEmail error at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                };
            }
        }

    }
}