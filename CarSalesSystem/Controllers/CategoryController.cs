using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
