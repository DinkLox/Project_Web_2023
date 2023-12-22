using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WEB_2023.Areas.Admin.Services;

namespace WEB_2023.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class BlogsController : Controller
    {
        private readonly ILogger<BlogsController> _logger;
        private readonly IBlogsManagerService _blogsManagerService;

        public BlogsController(ILogger<BlogsController> logger, IBlogsManagerService blogsManagerService)
        {
            _logger = logger;
            _blogsManagerService = blogsManagerService;
        }
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            var response = await _blogsManagerService.GetBlogsAsync(page, pageSize);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound();
        }

    }
}

