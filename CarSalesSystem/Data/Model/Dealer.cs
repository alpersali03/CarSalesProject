using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class Dealer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? CompanyName { get; set; } 

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }

}
