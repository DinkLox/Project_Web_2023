using System.Web;
using Microsoft.AspNetCore.Mvc;
using WEB_2023.Models;
using WEB_2023.Services;

namespace WEB_2023.Controllers
{
	[Route("[controller]")]
	public class ProductsController : Controller
	{
		public readonly ILogger<ProductsController> _logger;
		public readonly IProductService _productService;
		public ProductsController(ILogger<ProductsController> logger, IProductService productService)
		{
			_logger = logger;
			_productService = productService;
		}
		public async Task<IActionResult> Index(string? categoryName, int? page, int? pageSize)
		{
			var decodeName = HttpUtility.UrlDecode(categoryName);
			var response = await _productService.GetProductsAsync(decodeName, page, pageSize);
			if (!response.IsSuccess)
			{
				_logger.LogError(response.Message);
				return NotFound();
			}
			var data = response.Data as dynamic;
			ViewBag.CategoryName = categoryName ?? "Tất cả sản phẩm";
			ViewBag.CurrentPage = data?.currentPage;
			ViewBag.TotalPages = data?.totalPages;
			ViewBag.CurrentPageSize = data?.currentPageSize;
			return View(data?.products as List<ProductViewModel>);
		}
		[HttpGet]
		[Route("{id}/{slug}")]
		public async Task<IActionResult> Detail(int id)
		{
			var response = await _productService.GetProductByIdAsync(id);
			if (!response.IsSuccess)
			{
				_logger.LogError(response.Message);
				return NotFound();
			}
			return View(response.Data as ProductViewModel);
		}
	}

}
