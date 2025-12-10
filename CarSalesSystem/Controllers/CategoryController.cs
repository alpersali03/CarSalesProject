using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Services;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace CarSalesSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;
        public CategoryController(ApplicationDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
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

            _categoryService.Add(dto);
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

           _categoryService.Edit(id, dto);

            return RedirectToAction("GetAll");
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
