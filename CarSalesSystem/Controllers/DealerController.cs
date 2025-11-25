using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
    public class DealerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
