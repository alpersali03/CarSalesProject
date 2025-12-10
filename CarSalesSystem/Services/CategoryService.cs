using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public void Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(cat => cat.Id == id);
            if (category == null)
            {
                throw  new ArgumentException("This category does not exist!");
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
