using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Controllers
{
	public class ContactController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
