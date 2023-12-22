
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Slugify;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Models;
using WEB_2023.Utilities;

namespace WEB_2023.Areas.Admin.Services
{
    public interface IBlogsManagerService
    {
        public Task<ActionResponse> GetBlogsAsync(int? page, int? pageSize);
        public Task<ActionResponse> GetBlogByIdAsync(int id);
        public Task<ActionResponse> CreateBlogAsync(CreateBlogModel blogModel);
        public Task<ActionResponse> UpdateBlogAsync(int id, UpdateBlogModel blogModel);
        public Task<ActionResponse> DeleteBlogAsync(int id);
    }
    public class BlogsManagerService : IBlogsManagerService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<BlogsManagerService> _logger;
        public BlogsManagerService(IHttpContextAccessor httpContextAccessor, ApplicationDBContext context, ILogger<BlogsManagerService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _logger = logger;
        }
        public async Task<ActionResponse> GetBlogsAsync(int? page, int? pageSize)
        {
            try
            {
                var query = _context.Blogs.AsQueryable();
                var totalBlog = await query.CountAsync();
                var currentPage = page ?? 1;
                var currentPageSize = pageSize ?? 10;
                var totalPage = (int)Math.Ceiling((double)totalBlog / currentPageSize);
                var blogs = await query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize).Select(blog => new BlogViewModel
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Description = blog.Description,
                    Content = blog.Detail,
                    Image = blog.ImageURL,
                    CategoryName = blog.Category.Name,
                    Author = blog.CreatedBy.FullName,
                    CreatedAt = blog.CreatedAt,
                    ViewCount = blog.ViewCount,
                    IsActive = blog.IsActive
                }).ToListAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách bài viết thành công",
                    Data = new
                    {
                        totalBlog,
                        totalPage,
                        currentPageSize,
                        currentPage,
                        blogs
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blogs Manager Service - GetBlogsAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách bài viết"
                };
            }
        }
        public async Task<ActionResponse> GetBlogByIdAsync(int id)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy bài viết"
                    };
                }
                blog.ViewCount++;
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Lấy bài viết thành công",
                    Data = new BlogViewModel
                    {
                        Id = blog.Id,
                        Title = blog.Title,
                        Description = blog.Description,
                        Content = blog.Detail,
                        Image = blog.ImageURL,
                        CategoryName = blog.Category.Name,
                        Author = blog.CreatedBy.FullName,
                        CreatedAt = blog.CreatedAt,
                        ViewCount = blog.ViewCount,
                        IsActive = blog.IsActive
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blogs Manager Service - GetBlogByIdAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy bài viết"
                };
            }
        }
        public async Task<ActionResponse> CreateBlogAsync(CreateBlogModel blogModel)
        {
            try
            {
                SlugHelper helper = new SlugHelper();
                string userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Bạn cần đăng nhập để thực hiện chức năng này"
                    };
                }
                var blogImage = UploadImage.UploadSingleImage(blogModel.Image) ?? string.Empty;
                var blog = new Blog
                {
                    Title = blogModel.Title,
                    Description = blogModel.Description,
                    Detail = blogModel.Content,
                    ImageURL = blogImage,
                    Alias = helper.GenerateSlug(blogModel.Title),
                    CategoryId = blogModel.CategoryId,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    ViewCount = 0,
                    IsActive = blogModel.IsActive
                };
                await _context.Blogs.AddAsync(blog);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Thêm bài viết thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blogs Manager Service - CreateBlogAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thêm bài viết"
                };
            }
        }
        public async Task<ActionResponse> UpdateBlogAsync(int id, UpdateBlogModel blogModel)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy bài viết"
                    };
                }
                SlugHelper helper = new SlugHelper();
                blog.Title = blogModel.Title ?? blog.Title;
                blog.Description = blogModel.Description;
                blog.Detail = blogModel.Content ?? blog.Detail;
                blog.ImageURL = UploadImage.UploadSingleImage(blogModel.Image) ?? blog.ImageURL;
                blog.Alias = blogModel.Title != blog.Title ? helper.GenerateSlug(blogModel.Title) : blog.Alias;
                blog.CategoryId = blogModel.CategoryId ?? blog.CategoryId;
                blog.IsActive = blogModel.IsActive ?? blog.IsActive;
                var result = _context.Update(blog);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật bài viết thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blogs Manager Service - UpdateBlogAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật bài viết"
                };
            }
        }
        public async Task<ActionResponse> DeleteBlogAsync(int id)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return new ActionResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy bài viết"
                    };
                }
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
                return new ActionResponse
                {
                    IsSuccess = true,
                    Message = "Xóa bài viết thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blogs Manager Service - DeleteBlogAsync at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                return new ActionResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa bài viết"
                };
            }
        }

    }
}