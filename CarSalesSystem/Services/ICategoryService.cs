using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
    public interface ICategoryService
    {
        void Add(CategoryDto dto);
        void Edit(int id, CategoryDto dto);

        void Delete(int id);

        
    }
}
