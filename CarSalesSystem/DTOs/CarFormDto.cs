using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
	public class CarFormDto
	{
		public int Id { get; set; }

		[Required]
		public string Brand { get; set; } = null!;

		[Required]
		public string Model { get; set; } = null!;

		[Required]
		public string Description { get; set; } = null!;

		[Required]
		[Url]
		public string ImageUrl { get; set; } = null!;

		[Range(1950, 2100)]
		public int Year { get; set; }

		[Range(0, int.MaxValue)]
		public int Mileage { get; set; }

		[Required]
		public string FuelType { get; set; } = null!;

		[Required]
		public string Transmission { get; set; } = null!;

		[Range(typeof(decimal), "0", "1000000000")]
		public decimal Price { get; set; }

		[Required]
		public string Country { get; set; } = null!;

		[Required]
		public string City { get; set; } = null!;

		[Range(1, int.MaxValue)]
		public int CategoryId { get; set; }

		public bool IsListed { get; set; }

		public string? UserId { get; set; }
	}
}
