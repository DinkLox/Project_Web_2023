using WEB_2023.Utilities;
using WEB_2023.Data;
using Microsoft.EntityFrameworkCore;
using WEB_2023.Models.Category;
using WEB_2023.Models;
using WEB_2023.Entities;
namespace WEB_2023.Services
{
    public interface IProductService
    {
        Task<ActionResponse> GetCategoriesAsync();
        Task<ActionResponse> GetCategoryByIdAsync(int id);
        Task<ActionResponse> GetHotProductsAsync(int quantity);
        Task<ActionResponse> GetDiscountProductsAsync(int quantity);
        Task<ActionResponse> GetProductByIdAsync(int id);
        Task<ActionResponse> GetProductsAsync(string? categoryName, int? page, int? pageSize);
    }
    public class ProductService : IProductService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ApplicationDBContext context, ILogger<ProductService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<ActionResponse> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories.Select(category => new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Alias = category.Alias,
                    Description = category.Description,
                    CreatedByName = category.CreatedBy.FullName,
                    CreatedAt = category.CreatedAt,
                    IsActive = category.IsActive
                }).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetCategoriesAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh mục"
                };
            }
        }
        public async Task<ActionResponse> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories.Select(category => new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Alias = category.Alias,
                    Description = category.Description,
                    CreatedByName = category.CreatedBy.FullName,
                    CreatedAt = category.CreatedAt,
                    IsActive = category.IsActive
                }).FirstOrDefaultAsync(category => category.Id == id);
                if (category == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy danh mục"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Data = category
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetCategoryByIdAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh mục theo id"
                };
            }

        }
        public async Task<ActionResponse> GetHotProductsAsync(int quantity)
        {
            try
            {
                var products = await _context.Products.Where(product => product.IsActive)
                .Where(product => product.IsHot == true)
                .Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ImageURL = product.ImageURL,
                    Alias = product.Alias,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    CategoryName = product.Category.Name,
                    CreatedAt = product.CreatedAt,
                    IsActive = product.IsActive
                }).Take(quantity).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy sản phẩm hot thành công",
                    Data = products
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetHotProductsAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy sản phẩm hot"
                };
            }

        }
        public async Task<ActionResponse> GetDiscountProductsAsync(int quantity)
        {
            try
            {
                var products = await _context.Products.Where(product => product.IsActive)
                .Where(product => product.DiscountPrice < product.Price)
                .Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ImageURL = product.ImageURL,
                    Alias = product.Alias,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    CategoryName = product.Category.Name,
                    CreatedAt = product.CreatedAt,
                    IsActive = product.IsActive
                }).Take(quantity).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy sản phẩm giảm giá thành công",
                    Data = products
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetDiscountProductsAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy sản phẩm giảm giá"
                };
            }

        }
        public async Task<ActionResponse> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.Where(product => product.IsActive)
                .Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Alias = product.Alias,
                    ImageURL = product.ImageURL,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    Detail = product.Detail,
                    CategoryName = product.Category.Name,
                    IsActive = product.IsActive
                }).FirstOrDefaultAsync(product => product.Id == id);
                if (product == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm"
                    };
                }
                return new ActionResponse
                {
                    IsSuccess = true,
                    Data = product
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetProductByIdAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy sản phẩm theo id"
                };
            }

        }
        public async Task<ActionResponse> GetProductsAsync(string? categoryName, int? page, int? pageSize)
        {
            try
            {
                var query = _context.Products.AsQueryable();
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    query = query.Where(product => product.Category.Name.ToLower() == categoryName.ToLower());
                }
                var totalProducts = await query.CountAsync();
                if (totalProducts == 0)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm"
                    };
                }
                var totalPages = (int)Math.Ceiling((double)totalProducts / currentPageSize);
                var products = query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize)
                .Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ImageURL = product.ImageURL,
                    Alias = product.Alias,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    CategoryName = product.Category.Name,
                    CreatedAt = product.CreatedAt,
                    IsActive = product.IsActive
                }).ToList();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy sản phẩm thành công",
                    Data = new
                    {
                        totalPages,
                        currentPage,
                        currentPageSize,
                        totalProducts,
                        products
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prouct Service - GetProductsAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy sản phẩm"
                };
            }
        }
    }
}
