using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Services;

namespace CarSalesSystem.Controllers
{
	public class CarController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ICarService _carService;

		public CarController(ApplicationDbContext context, ICarService carService)
		{
			_context = context;
			_carService = carService;
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
						City = c.City
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
				ViewBag.Categories = _context.Categories.ToList();
				ViewBag.Dealers = _context.Dealers.ToList();
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

				var dto = new CarFormDto
				{
					Brand = car.Brand,
					Model = car.Model,
					Description = car.Description,
					ImageUrl = car.ImageUrl,
					Year = car.Year,
					Mileage = car.Mileage,
					FuelType = car.FuelType,
					Transmission = car.Transmission,
					Price = car.Price,
					Country = car.Country,
					City = car.City,
					CategoryId = car.CategoryId,
					DealerId = car.DealerId
				};

				return View(dto);
			}
			catch (Exception)
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
		public IActionResult SortByName()
		{
			try
			{
				var cars = _context.Cars
					.OrderBy(c => c.Brand)
					.ThenBy(c => c.Model)
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
				return BadRequest("Failed to sort by name.");
			}
		}

		[HttpGet]
		public IActionResult SortByPrice()
		{
			try
			{
				var cars = _context.Cars
					.OrderBy(c => c.Price)
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
	}
}
