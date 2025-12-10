using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public CategoryService(ApplicationDbContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public void Add(CategoryDto dto)
        {
           
            
            var category = new Category
            {
                Name = dto.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Edit(int id, CategoryDto dto)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                throw new ArgumentException("This category does not exist!");
            }
                

            category.Name = dto.Name;


            _context.SaveChanges();
        }
    }
}
