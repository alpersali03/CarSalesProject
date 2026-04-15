using CarSalesSystem.Data;
using CarSalesSystem.Extensions;
using CarSalesSystem.Models;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
	[Authorize]
	public class FavoriteController : Controller
	{
		private readonly IFavoriteService _favoriteService;
		private readonly ApplicationDbContext _context;

		public FavoriteController(IFavoriteService favoriteService, ApplicationDbContext context)
		{
			_favoriteService = favoriteService;
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			var viewModel = new CarCatalogViewModel
			{
				Cars = _favoriteService.GetFavoriteCars(userId),
				FavoriteCarIds = _favoriteService.GetFavoriteCarIds(userId),
				Brands = _context.Cars.Where(c => !c.IsBought && c.IsListed).Select(c => c.Brand).Distinct().OrderBy(b => b).ToList(),
				Categories = _context.Categories.Select(c => new DTOs.CategoryDto { Id = c.Id, Name = c.Name }).ToList(),
				Search = new CarSearchViewModel { FavoritesOnly = true }
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Toggle(int carId, string? returnUrl = null)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			_favoriteService.ToggleFavorite(userId, carId);

			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("GetAll", "Car");
		}
	}
}
