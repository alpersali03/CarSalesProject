using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }


}
