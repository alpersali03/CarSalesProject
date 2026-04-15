using CarSalesSystem.Data;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Models;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarSalesSystem.Controllers
{
	public class CarController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ICarService _carService;
		private readonly IDealerService _dealerService;
		private readonly IFavoriteService _favoriteService;

		public CarController(
			ApplicationDbContext context,
			ICarService carService,
			IDealerService dealerService,
			IFavoriteService favoriteService)
		{
			_context = context;
			_carService = carService;
			_dealerService = dealerService;
			_favoriteService = favoriteService;
		}

		[HttpGet]
		public IActionResult GetAll([FromQuery] CarSearchViewModel search)
		{
			try
			{
				var favoriteIds = _favoriteService.GetFavoriteCarIds(User.GetId());
				var cars = search.FavoritesOnly && User.Identity?.IsAuthenticated == true
					? _favoriteService.GetFavoriteCars(User.GetId()!)
					: _carService.Search(search);

				var viewModel = BuildCatalogViewModel(search, cars, favoriteIds);
				return View(viewModel);
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading cars.");
			}
		}

		[HttpGet]
		public IActionResult Compare(string ids)
		{
			var carIds = ParseIds(ids);
			var cars = _carService.GetByIds(carIds);
			return View(cars);
		}

		[HttpGet]
		[Authorize]
		public IActionResult Add()
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId) || !_dealerService.CheckIsDealerByUserId(userId))
			{
				return Forbid();
			}

			ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
			return View(new CarFormDto());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Add(CarFormDto dto)
		{
			ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();

			if (!ModelState.IsValid)
			{
				return View(dto);
			}

			try
			{
				var userId = User.GetId();
				if (string.IsNullOrWhiteSpace(userId))
				{
					return Challenge();
				}

				dto.UserId = userId;
				_carService.Add(dto);
				return RedirectToAction(nameof(GetAll));
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
			catch (Exception)
			{
				return BadRequest("Failed to add car.");
			}
		}

		[HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			var dealer = _dealerService.GetDealerByUserId(userId);
			if (dealer == null)
			{
				return Forbid();
			}

			var car = _carService.GetById(id);
			if (car == null)
			{
				return NotFound();
			}

			if (car.DealerId != dealer.Id)
			{
				return Forbid();
			}

			ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", car.CategoryId);

			return View(new CarFormDto
			{
				Id = car.Id,
				Brand = car.Brand,
				Model = car.Model,
				Description = car.Description,
				ImageUrl = car.ImageUrl,
				Year = car.Year,
				Mileage = car.Mileage,
				FuelType = car.FuelType,
				Transmission = car.Transmission,
				Price = car.Price,
				IsListed = car.IsListed,
				Country = car.Country,
				City = car.City,
				CategoryId = car.CategoryId
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int id, CarFormDto dto)
		{
			ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", dto.CategoryId);

			if (!ModelState.IsValid)
			{
				return View(dto);
			}

			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			try
			{
				_carService.Edit(id, dto, userId);
				return RedirectToAction(nameof(GetAll));
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
			catch (ArgumentException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				return BadRequest("Failed to update car.");
			}
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			try
			{
				var car = _carService.GetById(id);
				if (car == null)
				{
					return NotFound();
				}

				ViewBag.IsFavorite = _favoriteService.GetFavoriteCarIds(User.GetId()).Contains(id);
				return View(car);
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading car details.");
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			try
			{
				_carService.Delete(userId, id);
				return RedirectToAction(nameof(GetAll));
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
			catch (ArgumentException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				return BadRequest("Failed to delete car.");
			}
		}

		[HttpGet]
		public IActionResult SortByName(string letter)
		{
			var search = new CarSearchViewModel();
			var viewModel = BuildCatalogViewModel(search, _carService.SortByName(letter), _favoriteService.GetFavoriteCarIds(User.GetId()));
			return View("GetAll", viewModel);
		}

		[HttpGet]
		public IActionResult SortByPrice(string sortOrder)
		{
			var search = new CarSearchViewModel();
			var viewModel = BuildCatalogViewModel(search, _carService.SortByPrice(sortOrder), _favoriteService.GetFavoriteCarIds(User.GetId()));
			return View("GetAll", viewModel);
		}

		[HttpGet]
		public IActionResult CarsByCategory(int categoryId)
		{
			return RedirectToAction(nameof(GetAll), new CarSearchViewModel { CategoryId = categoryId });
		}

		[HttpGet]
		public IActionResult GetByFuel(string fuelType)
		{
			return RedirectToAction(nameof(GetAll), new CarSearchViewModel { FuelType = fuelType });
		}

		[HttpGet]
		public IActionResult GetByYear(int? minYear, int? maxYear)
		{
			return RedirectToAction(nameof(GetAll), new CarSearchViewModel { MinYear = minYear, MaxYear = maxYear });
		}

		[HttpGet]
		public IActionResult GetByBrand(string brandType)
		{
			return RedirectToAction(nameof(GetAll), new CarSearchViewModel { BrandType = brandType });
		}

		[HttpGet]
		public IActionResult Search([FromQuery] CarSearchViewModel search)
		{
			return RedirectToAction(nameof(GetAll), search);
		}

		private CarCatalogViewModel BuildCatalogViewModel(CarSearchViewModel search, List<CarDto> cars, HashSet<int> favoriteIds)
		{
			return new CarCatalogViewModel
			{
				Cars = cars,
				Search = search,
				Brands = _carService.PopulateBrands(),
				Categories = _context.Categories
					.Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
					.OrderBy(c => c.Name)
					.ToList(),
				FavoriteCarIds = favoriteIds,
				CurrentDealerId = GetCurrentDealerId(),
				CanManageCars = User.Identity?.IsAuthenticated == true
			};
		}

		private int? GetCurrentDealerId()
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return null;
			}

			return _dealerService.GetDealerByUserId(userId)?.Id;
		}

		private static List<int> ParseIds(string? ids)
		{
			if (string.IsNullOrWhiteSpace(ids))
			{
				return [];
			}

			return ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
				.Where(id => id.HasValue)
				.Select(id => id!.Value)
				.Distinct()
				.Take(4)
				.ToList();
		}
	}
}
