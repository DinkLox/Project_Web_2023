using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_2023.Areas.Admin.Models;
using WEB_2023.Areas.Admin.Services;
using WEB_2023.Config;
using WEB_2023.Entities;
using WEB_2023.Models;
using WEB_2023.Models.Category;
using WEB_2023.Services;

namespace WEB_2023.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly IProductManagerService _productManagerService;
        private readonly ICategoriesManagerService _categoriesManagerService;
        public ProductsController(ILogger<ProductsController> logger, IProductService productService, ICategoriesManagerService categoriesManagerService, IProductManagerService productManagerService)
        {
            _logger = logger;
            _productService = productService;
            _categoriesManagerService = categoriesManagerService;
            _productManagerService = productManagerService;
        }
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _productManagerService.GetProductsAsync(page, pageSize);
            if (response.IsSuccess)
            {
                var data = response.Data as dynamic;
                ViewBag.TotalProduct = data?.totalProduct;
                ViewBag.TotalPage = data?.totalPage;
                ViewBag.currentPageSize = data?.currentPageSize;
                ViewBag.CurrentPage = data?.currentPage;
                return View(data?.products as List<ProductViewModel>);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("View/{id}")]
        public async Task<IActionResult> ViewProduct(int id)
        {
            var response = await _productManagerService.GetProductByIdAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> Categories()
        {
            var response = await _productService.GetCategoriesAsync();
            if (response.IsSuccess)
            {
                return View(response.Data as List<CategoryViewModel>);
            }
            return StatusCode(500);
        }
        [HttpPost]
        [Route("Create/Category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel newCategory)
        {
            if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Danh mục nhập vào không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("Categories");
            }
            var response = await _categoriesManagerService.CreateCategoryAsync(newCategory);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Route("Update/Category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel updateCategory)
        {
            if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Danh mục nhập vào không hợp lệ";
                TempData["Type"] = "warning";
                return RedirectToAction("Categories");
            }
            var response = await _categoriesManagerService.UpdateCategoryAsync(updateCategory);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Route("Hide/Category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HideCategory(int id)
        {
            var response = await _categoriesManagerService.HideCategoryAsync(id);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Route("Show/Category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowCategory(int id)
        {
            var response = await _categoriesManagerService.ShowCategoryAsync(id);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Route("Delete/Category")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoriesManagerService.DeleteCategoryAsync(id);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Categories");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Categories");
        }
        [HttpGet]
        [Route("Create-Product")]
        public async Task<IActionResult> CreateProduct()
        {
            var response = await _productService.GetCategoriesAsync();
            if (response.IsSuccess)
            {
                var Categories = response.Data as List<CategoryViewModel>;
                if (Categories != null)
                {
                    ViewBag.Categories = new SelectList(Categories.ToList(), "Id", "Name");
                }
                return View();
            }
            return StatusCode(500);
        }
        [HttpPost]
        [Route("Create/Product")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductModel createProductModel)
        {
            /*if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Sản phẩm nhập vào không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("Index");
            }*/
            var response = await _productManagerService.CreateProductAsync(createProductModel);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("{id}/Update")]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var response = await _productManagerService.GetProductByIdAsync(id);
            if (response.IsSuccess)
            {
                var product = response.Data as ProductViewModel;
                var categories = await _productService.GetCategoriesAsync();
                if (Categories != null && product != null)
                {
                    ViewBag.Categories = new SelectList(categories.Data as List<CategoryViewModel>, "Id", "Name", product.CategoryId);
                    return View((product, new UpdateProductModel()));
                }
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("{id}/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct([Bind(Prefix = "Item2")] UpdateProductModel updateProductModel, int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["SystemMessage"] = "Sản phẩm nhập vào không hợp lệ";
                TempData["Type"] = "error";
                return RedirectToAction("Index");
            }
            var response = await _productManagerService.UpdateProductAsync(updateProductModel, id);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Delete/Product/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productManagerService.DeleteProductAsync(id);
            if (response.IsSuccess)
            {
                TempData["SystemMessage"] = response.Message;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            TempData["SystemMessage"] = response.Message;
            TempData["Type"] = "error";
            return RedirectToAction("Index");
        }
    }
}