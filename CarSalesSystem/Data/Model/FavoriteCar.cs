using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSalesSystem.Data.Model
{
	public class FavoriteCar
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = null!;

		[Required]
		public int CarId { get; set; }

		[ForeignKey(nameof(CarId))]
		public Car Car { get; set; } = null!;

		public DateTime CreatedOnUtc { get; set; }
	}
}
