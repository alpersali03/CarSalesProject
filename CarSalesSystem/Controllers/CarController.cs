using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace CarSalesSystem.Controllers
{

	public class CarController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ICarService _carService;
		private readonly IDealerService _dealerService;


		public CarController(ApplicationDbContext context, ICarService carService, IDealerService dealerService)
		{
			_context = context;
			_carService = carService;
			_dealerService = dealerService;
		}


		[HttpGet]
		public IActionResult GetAll()
		{
			PopulateBrands();
			try
			{
				var cars = _context.Cars
					.Include(c => c.Category)
					.Include(c => c.Dealer)
					.Select(c => new CarDto
					{
						Id = c.Id,
						Brand = c.Brand,
						Model = c.Model,
						Price = c.Price,
						ImageUrl = c.ImageUrl,
						City = c.City,
						Year = c.Year,
						FuelType = c.FuelType,
						Mileage = c.Mileage
					})
					.ToList();

				return View(cars);
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading cars.");
			}
		}

		[HttpGet]
		public IActionResult Add()
		{
			PopulateBrands();
			try
			{
				var userId = User.GetId();

				if (!_dealerService.CheckIsDealerByUserId(userId))
				{
					return Forbid();
				}

				ViewBag.Categories = _context.Categories.ToList();
				return View(new CarFormDto());
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading form data.");
			}
		}

		[HttpPost]
		public IActionResult Add(CarFormDto dto)
		{
			PopulateBrands();
			if (!ModelState.IsValid)
				return View(dto);

			try
			{
				var getUserId = User.GetId();
				dto.UserId = getUserId;
				if (getUserId == null)
				{
					return NotFound();
				}
				_carService.Add(dto);
				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return BadRequest("Failed to add car.");
			}
		}
		[HttpGet]
		public IActionResult Edit(int id)
		{
			PopulateBrands();
			try
			{
				var car = _carService.GetById(id);
				if (car == null)
					return NotFound();


				ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", car.CategoryId);
				ViewBag.DealerId = new SelectList(_context.Dealers, "Id", "Name", car.DealerId);

				var dto = new CarFormDto
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
					CategoryId = car.CategoryId,
					DealerId = car.DealerId
				};

				return View(dto);
			}
			catch
			{
				return BadRequest("An error occurred while loading car details.");
			}
		}


		[HttpPost]
		public IActionResult Edit(int id, CarFormDto dto)
		{
			PopulateBrands();
			if (!ModelState.IsValid)
				return View(dto);

			try
			{
				_carService.Edit(id, dto);
				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return BadRequest("Failed to update car.");
			}
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			PopulateBrands();
			try
			{
				
				var car = _carService.GetById(id);

				if (car == null)
					return NotFound();

				return View(car);
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading car details.");
			}
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			var userId = User.GetId();
			try
			{
				_carService.Delete(userId, id);
				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return BadRequest("Failed to delete car.");
			}
		}

		[HttpGet]
		public IActionResult SortByName(string letter)
		{
			PopulateBrands();
			try
			{
				var cars = _carService.SortByName(letter);
				return View("GetAll", cars);
			}
			catch (Exception)
			{
				return BadRequest("Failed to sort by name.");
			}
		}

		[HttpGet]
		public IActionResult SortByPrice(string sortOrder)
		{
			PopulateBrands();
			try
			{
				var cars = _carService.SortByPrice(sortOrder);
				return View("GetAll", cars);
			}
			catch (Exception)
			{
				return BadRequest("Failed to sort by price.");
			}
		}

		[HttpGet]
		public IActionResult CarsByCategory(int categoryId)
		{
			PopulateBrands();
			try
			{
				var cars = _context.Cars
					.Where(c => c.CategoryId == categoryId)
					.Select(c => new CarDto
					{
						Id = c.Id,
						Brand = c.Brand,
						Model = c.Model,
						Price = c.Price,
						ImageUrl = c.ImageUrl,
						City = c.City
					})
					.ToList();

				return View("GetAll", cars);
			}
			catch (Exception)
			{
				return BadRequest("Failed to filter cars by category.");
			}
		}
		
		[HttpGet]
		public IActionResult GetByFuel(string fuelType)
		{
			PopulateBrands();
			var cars = _carService.GetByFuel(fuelType);
			try
			{
				if (fuelType == null)
				{
					return BadRequest("Fuel type is required.");
				}

			}

			catch (Exception)
			{
				return BadRequest("Failed to filter cars by fuel type.");

			}
			return View("GetAll", cars);
		}
		[HttpGet]
		public IActionResult GetByYear(int? minYear, int? maxYear)
		{
			PopulateBrands();
			try
			{
				var cars = _carService.GetByYear(minYear, maxYear);

				return View("GetAll", cars);
			}
			catch (Exception)
			{
				return BadRequest("Failed to filter cars by year range.");
			}
		}
		[HttpGet]
		public IActionResult GetByBrand(string brandType)
		{
			PopulateBrands();
			try
			{
				var cars = _carService.GetByBrand(brandType);
				return View("GetAll", cars);
			}
			catch (Exception)
			{
				return BadRequest("Failed to filter cars by brand.");
			}
		}
		[HttpGet]
		public IActionResult Search(int? minYear, int? maxYear, string? fuelType, string? brandType)
		{
			PopulateBrands();
			try
			{
				List<CarDto> cars = _carService.Search(minYear, maxYear, fuelType, brandType);
				return View("GetAll", cars);

			}

			catch (Exception)
			{
				return BadRequest("Failed to perform search.");
			}
		}
		private void PopulateBrands()
		{
			ViewData["Brands"] = _context.Cars
				.Select(c => c.Brand)
				.Distinct()
				.OrderBy(b => b)
				.ToList();
		}
	}
}
