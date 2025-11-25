using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var cars = _context.Cars
                .Include(c => c.Category)
                .Include(c => c.Dealer)
                .Select(c => new CatDto
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

        [HttpGet]
        public IActionResult Add()
        {
            return View(new CarFormDto());
        }

        
        [HttpPost]
        public IActionResult Add(CarFormDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var car = new Car
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Year = dto.Year,
                Mileage = dto.Mileage,
                FuelType = dto.FuelType,
                Transmission = dto.Transmission,
                Price = dto.Price,
                Country = dto.Country,
                City = dto.City,
                CategoryId = dto.CategoryId,
                DealerId = dto.DealerId,
                IsListed = true
            };

            _context.Cars.Add(car);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAll));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }
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

        
        [HttpPost]
        public IActionResult Edit(int id, CarFormDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();

            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Description = dto.Description;
            car.ImageUrl = dto.ImageUrl;
            car.Year = dto.Year;
            car.Mileage = dto.Mileage;
            car.FuelType = dto.FuelType;
            car.Transmission = dto.Transmission;
            car.Price = dto.Price;
            car.Country = dto.Country;
            car.City = dto.City;
            car.CategoryId = dto.CategoryId;
            car.DealerId = dto.DealerId;

            _context.SaveChanges();

            return RedirectToAction(nameof(GetAll));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var car = _context.Cars
                .Include(c => c.Category)
                .Include(c => c.Dealer)
                .FirstOrDefault(c => c.Id == id);

            if (car == null)
                return NotFound();

            return View(car); 
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            _context.SaveChanges();

            return RedirectToAction(nameof(GetAll));
        }

        [HttpGet]
        public IActionResult SortByName()
        {
            var cars = _context.Cars
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new CatDto
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

        [HttpGet]
        public IActionResult SortByPrice()
        {
            var cars = _context.Cars
                .OrderBy(c => c.Price)
                .Select(c => new CatDto
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

        [HttpGet]
        public IActionResult CarsByCategory(int categoryId)
        {
            var cars = _context.Cars
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new CatDto
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
    }
}
