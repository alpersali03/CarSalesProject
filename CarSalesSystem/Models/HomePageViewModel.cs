using CarSalesSystem.DTOs;

namespace CarSalesSystem.Models
{
	public class HomePageViewModel
	{
		public CarSearchViewModel Search { get; set; } = new();

		public List<CarDto> FeaturedCars { get; set; } = new();

		public List<string> Brands { get; set; } = new();

		public List<CategoryDto> Categories { get; set; } = new();

		public List<DealerSummaryViewModel> TopDealers { get; set; } = new();

		public int ActiveCarsCount { get; set; }

		public int DealersCount { get; set; }

		public int SoldCarsCount { get; set; }

		public HashSet<int> FavoriteCarIds { get; set; } = new();
	}
}
