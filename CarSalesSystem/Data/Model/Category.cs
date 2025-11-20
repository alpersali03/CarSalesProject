using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class Category
    {
        /// <summary>
        /// category id 
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// name of the category 
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// relation of the tables 
        /// </summary>
        public IEnumerable<Car>? Cars { get; init; } = new List<Car>();
    }
}
