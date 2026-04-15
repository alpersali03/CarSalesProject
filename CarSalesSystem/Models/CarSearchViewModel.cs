namespace CarSalesSystem.Models
{
	public class CarSearchViewModel
	{
		public string? FuelType { get; set; }

		public string? BrandType { get; set; }

		public int? MinYear { get; set; }

		public int? MaxYear { get; set; }

		public decimal? MinPrice { get; set; }

		public decimal? MaxPrice { get; set; }

		public int? CategoryId { get; set; }

		public string? City { get; set; }

		public bool FavoritesOnly { get; set; }
	}
}
