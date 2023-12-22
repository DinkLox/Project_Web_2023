using System.Security.Claims;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Utilities;

namespace WEB_2023.Areas.Admin.Services
{
    public interface ICategoriesManagerService
    {
        Task<ActionResponse> CreateCategoryAsync(CreateCategoryModel newCategory);
        Task<ActionResponse> UpdateCategoryAsync(UpdateCategoryModel updateCategory);
        Task<ActionResponse> HideCategoryAsync(int id);
        Task<ActionResponse> ShowCategoryAsync(int id);
        Task<ActionResponse> DeleteCategoryAsync(int id);
    }
    public class CategoriesManagerService : ICategoriesManagerService
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoriesManagerService> _logger;
        public CategoriesManagerService(ApplicationDBContext context, ILogger<CategoriesManagerService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<ActionResponse> CreateCategoryAsync(CreateCategoryModel newCategory)
        {
            try
            {
                string UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var category = new Category
                {
                    Name = newCategory.Name,
                    Description = newCategory.Description,
                    CreatedById = UserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = newCategory.IsActive
                };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Thêm danh mục thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Categories Manager Service - CreateCategoryAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thêm danh mục"
                };
            }
        }
        public async Task<ActionResponse> UpdateCategoryAsync(UpdateCategoryModel updateCategory)
        {
            try
            {
                var category = await _context.Categories.FindAsync(updateCategory.Id);
                if (category == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy danh mục"
                    };
                }
                category.Name = updateCategory.Name;
                category.Description = updateCategory.Description;
                var result = _context.Update(category);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật thông tin danh mục thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Categories Manager Service - UpdateCategoryAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi cập nhật danh mục"
                };
            }
        }
        public async Task<ActionResponse> HideCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy danh mục"
                    };
                }
                category.IsActive = false;
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Ẩn danh mục thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Categories Manager Service - HideCategoryAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi ẩn danh mục"
                };
            }
        }
        public async Task<ActionResponse> ShowCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy danh mục"
                    };
                }
                category.IsActive = true;
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Hiển thị danh mục thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Categories Manager Service - ShowCategoryAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi hiển thị danh mục"
                };
            }
        }
        public async Task<ActionResponse> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy danh mục"
                    };
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Xóa danh mục thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Categories Manager Service - DeleteCategoryAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi xóa danh mục"
                };
            }
        }
    }

}