using CarSalesSystem.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
    public class DealerDto
    {
        public int Id { get; set; }

       
        public string Name { get; set; } = null!;

        public string? CompanyName { get; set; }

    
        public string PhoneNumber { get; set; } = null!;

      
        public string? UserId { get; set; }

		public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }
}
