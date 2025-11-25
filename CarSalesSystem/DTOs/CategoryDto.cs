using CarSalesSystem.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
    public class CategoryDto
    {
        
        public int Id { get; set; }

        
        public string Name { get; set; } = null!;

        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }
}
