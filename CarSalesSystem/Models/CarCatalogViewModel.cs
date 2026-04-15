using CarSalesSystem.DTOs;

namespace CarSalesSystem.Models
{
	public class CarCatalogViewModel
	{
		public List<CarDto> Cars { get; set; } = new();

		public CarSearchViewModel Search { get; set; } = new();

		public List<string> Brands { get; set; } = new();

		public List<CategoryDto> Categories { get; set; } = new();

		public HashSet<int> FavoriteCarIds { get; set; } = new();

		public bool CanManageCars { get; set; }

		public int? CurrentDealerId { get; set; }
	}
}
