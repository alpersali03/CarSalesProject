using CarSalesSystem.Data;
using CarSalesSystem.Extensions;
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
		private readonly IFavoriteService _favoriteService;
		private readonly ApplicationDbContext _context;

		public HomeController(
			ILogger<HomeController> logger,
			ICarService carService,
			IFavoriteService favoriteService,
			ApplicationDbContext context)
		{
			_logger = logger;
			_carService = carService;
			_favoriteService = favoriteService;
			_context = context;
		}

		public IActionResult Index()
		{
			var favoriteIds = _favoriteService.GetFavoriteCarIds(User.GetId());
			var dealerRevenue = _context.Payments
				.Select(p => new { p.Car.DealerId, p.TotalAmount })
				.ToList()
				.GroupBy(p => p.DealerId)
				.ToDictionary(g => g.Key, g => g.Sum(p => p.TotalAmount));

			var topDealers = _context.Dealers
				.Select(d => new DealerSummaryViewModel
				{
					Id = d.Id,
					Name = d.Name,
					CompanyName = d.CompanyName,
					PhoneNumber = d.PhoneNumber,
					ActiveListingsCount = d.Cars.Count(c => c.IsListed && !c.IsBought),
					SoldCarsCount = d.Cars.Count(c => c.IsBought)
				})
				.OrderByDescending(d => d.ActiveListingsCount)
				.Take(3)
				.ToList();

			foreach (var dealer in topDealers)
			{
				dealer.Revenue = dealerRevenue.GetValueOrDefault(dealer.Id);
			}

			var viewModel = new HomePageViewModel
			{
				FeaturedCars = _carService.GetLatest(8),
				Brands = _carService.PopulateBrands(),
				Categories = _context.Categories
					.Select(c => new DTOs.CategoryDto { Id = c.Id, Name = c.Name })
					.OrderBy(c => c.Name)
					.ToList(),
				ActiveCarsCount = _context.Cars.Count(c => !c.IsBought && c.IsListed),
				DealersCount = _context.Dealers.Count(),
				SoldCarsCount = _context.Cars.Count(c => c.IsBought),
				FavoriteCarIds = favoriteIds,
				TopDealers = topDealers
			};

			return View(viewModel);
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
