using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace CarSalesSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var category = _context.Categories.Include(cat=>cat.Cars).Select(cat => new CategoryDto
            {
                Id = cat.Id,
                Name = cat.Name,
            }).ToList();

            return View(category);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View(new CategoryDto());
        }
        [HttpPost]
        public IActionResult Add(CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var category = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("GetAll");

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if(category == null)
            {
                return NotFound();  
            }
            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
            return View(dto);
        }
        [HttpPost]
        public IActionResult Edit(int id, CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.Id = dto.Id;
            category.Name = dto.Name;


            _context.SaveChanges();

            return RedirectToAction(nameof(GetAll));
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(cat=>cat.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("GetAll");
        }
    }
}
