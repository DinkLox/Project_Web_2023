using Slugify;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Models;
using WEB_2023.Utilities;

namespace WEB_2023.Services
{
    public interface IProductManagerService
    {
        Task<ActionResponse> GetProductsAsync(int? pageIndex, int? pageSize);
        Task<ActionResponse> GetProductByIdAsync(int productId);
        Task<ActionResponse> CreateProductAsync(CreateProductModel product);
        Task<ActionResponse> UpdateProductAsync(UpdateProductModel product, int productId);
        Task<ActionResponse> DeleteProductAsync(int productId);
    }
    public class ProductManagerService : IProductManagerService
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductManagerService> _logger;
        public ProductManagerService(ApplicationDBContext context, ILogger<ProductManagerService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ActionResponse> GetProductsAsync(int? pageIndex, int? pageSize)
        {
            try
            {
                var query = _context.Products.AsQueryable();
                var totalProduct = await query.CountAsync();
                var currentPageSize = pageSize ?? 10;
                var currentPage = pageIndex ?? 1;
                var totalPage = (int)Math.Ceiling((double)totalProduct / currentPageSize);
                var products = await query.Skip((currentPage - 1) * currentPageSize)
                                        .Take(currentPageSize)
                                        .Select(product => new ProductViewModel
                                        {
                                            Id = product.Id,
                                            Name = product.Name,
                                            Alias = product.Alias,
                                            Description = product.Description,
                                            ImageURL = product.ImageURL,
                                            Price = product.Price,
                                            CategoryId = product.CategoryId,
                                            CategoryName = product.Category.Name,
                                            CreatedByName = product.CreatedBy.FullName,
                                            CreatedAt = product.CreatedAt,
                                            IsActive = product.IsActive,
                                        }).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = new
                    {
                        totalProduct,
                        totalPage,
                        currentPageSize,
                        currentPage,
                        products
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Manager Service - GetProductsAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy danh sách sản phẩm"
                };
            }
        }
        public async Task<ActionResponse> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _context.Products.Where(product => product.Id == productId).FirstOrDefaultAsync();
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
                    Message = "Lấy sản phẩm thành công",
                    Data = new ProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Alias = product.Alias,
                        Description = product.Description,
                        Detail = product.Detail,
                        ImageURL = product.ImageURL,
                        Price = product.Price,
                        DiscountPrice = product.DiscountPrice,
                        CategoryId = product.CategoryId,
                        CategoryName = product.Category.Name,
                        CreatedByName = product.CreatedBy.FullName,
                        CreatedAt = product.CreatedAt,
                        IsActive = product.IsActive,
                        IsHot = product.IsHot
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Manager Service - GetProductByIdAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi lấy sản phẩm"
                };
            }
        }
        public async Task<ActionResponse> CreateProductAsync(CreateProductModel product)
        {
            try
            {
                SlugHelper helper = new SlugHelper();
                string userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (userId == string.Empty)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng"
                    };
                }
                var productImageURL = UploadImage.UploadSingleImage(product.ProductImage) ?? "/uploads/images/default-product.jpeg";
                var newProduct = new Product
                {
                    Name = product.Name,
                    Alias = helper.GenerateSlug(product.Name),
                    Description = product.Description,
                    Detail = product.Detail,
                    ImageURL = productImageURL,
                    Price = product.Price,
                    DiscountPrice = product.Discount,
                    CategoryId = product.CategoryId,
                    CreatedById = userId,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Thêm sản phẩm thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Manager Service - CreateProductAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi thêm sản phẩm"
                };
            }
        }
        public async Task<ActionResponse> UpdateProductAsync(UpdateProductModel product, int productId)
        {
            try
            {
                var productToUpdate = await _context.Products.FindAsync(productId);
                if (productToUpdate == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm"
                    };
                }
                SlugHelper helper = new SlugHelper();
                string? productImageURL = product.ProductImage != null ? UploadImage.UploadSingleImage(product.ProductImage) : productToUpdate.ImageURL;
                productToUpdate.Name = product.Name ?? productToUpdate.Name;
                productToUpdate.Alias = product.Name != null ? helper.GenerateSlug(product.Name) : productToUpdate.Alias;
                productToUpdate.Description = product.Description ?? productToUpdate.Description;
                productToUpdate.Detail = product.Detail ?? productToUpdate.Detail;
                productToUpdate.ImageURL = productImageURL ?? productToUpdate.ImageURL;
                productToUpdate.Price = product.Price ?? productToUpdate.Price;
                productToUpdate.DiscountPrice = product.Discount ?? productToUpdate.DiscountPrice;
                productToUpdate.CategoryId = product.CategoryId ?? productToUpdate.CategoryId;
                productToUpdate.IsActive = product.IsActive ?? productToUpdate.IsActive;
                var result = _context.Update(productToUpdate);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật sản phẩm thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Manager Service - UpdateProductAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi cập nhật sản phẩm"
                };
            }
        }
        public async Task<ActionResponse> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm"
                    };
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Xóa sản phẩm thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Manager Service - DeleteProductAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi khi xóa sản phẩm"
                };
            }
        }

    }
}