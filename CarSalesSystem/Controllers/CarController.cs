using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
namespace CarSalesSystem.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

    
        public IActionResult GetAll()
        {
            var cars = _context.Cars.Include(c => c.Category).Include(c => c.Dealer).ToList();
            return View(cars);
        }

    
        public IActionResult Add()
        {
            return View();
        }

      
        [HttpPost]
        public IActionResult Add(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);

            _context.Cars.Add(car);
            _context.SaveChanges();
            return RedirectToAction("GetAll");
        }


        public IActionResult Edit(int id)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();

            return View(car);
        }

        
        [HttpPost]
        public IActionResult Edit(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);

            _context.Cars.Update(car);
            _context.SaveChanges();
            return RedirectToAction("GetAll");
        }

        
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
            return RedirectToAction("GetAll");
        }

        public IActionResult SortByName()
        {
            var cars = _context.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList();
            return View("GetAll", cars);
        }


        public IActionResult SortByPrice()
        {
            var cars = _context.Cars.OrderBy(c => c.Price).ToList();
            return View("GetAll", cars);
        }

        public IActionResult CarsByCategory(int categoryId)
        {
            var cars = _context.Cars
                .Where(c => c.CategoryId == categoryId)
                .Include(c => c.Category)
                .ToList();

            return View("GetAll", cars);
        }
    }
}
