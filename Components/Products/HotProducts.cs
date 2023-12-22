
using Microsoft.AspNetCore.Mvc;
using WEB_2023.Models;
using WEB_2023.Services;

namespace WEB_2023.Components.Products
{
    [ViewComponent(Name = "HotProducts")]
    public class HotProducts : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly ILogger<HotProducts> _logger;

        public HotProducts(IProductService productService, ILogger<HotProducts> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(int quantity)
        {
            var response = await _productService.GetHotProductsAsync(quantity);
            if (!response.IsSuccess)
            {
                _logger.LogError(response.Message);
                return await Task.FromResult<IViewComponentResult>(View(new List<ProductViewModel>()));
            }
            return await Task.FromResult<IViewComponentResult>(View(response.Data as List<ProductViewModel>));
        }
    }
}