using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
    public class Car : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
