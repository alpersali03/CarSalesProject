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
			try
			{
				var car = _context.Cars
					.Include(c => c.Category)
					.Include(c => c.Dealer)
					.FirstOrDefault(c => c.Id == id);

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
			try
			{
				var car = _context.Cars.Find(id);
				if (car == null)
					return NotFound();

				_context.Cars.Remove(car);
				_context.SaveChanges();

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
		public IActionResult Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				TempData["Error"] = "Please enter a keyword to search.";
				return RedirectToAction("GetAll");
			}

			var product = _carService.Search(keyword);
			return View("GetAll", product);
		}
		[HttpGet]
		public IActionResult GetByFuel(string fuelType)
		{
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
	}
}
