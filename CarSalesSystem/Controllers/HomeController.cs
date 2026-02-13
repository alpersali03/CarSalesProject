using CarSalesSystem.Models;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarSalesSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly ICarService _carService;

		public HomeController(ILogger<HomeController> logger, ICarService carService)
		{
			_logger = logger;
			_carService = carService;
		}

		public IActionResult Index()
		{
			var cars = _carService.GetLatest(6);
			return View(cars);
		}


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
