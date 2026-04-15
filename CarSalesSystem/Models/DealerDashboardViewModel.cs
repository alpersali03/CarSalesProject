using CarSalesSystem.DTOs;

namespace CarSalesSystem.Models
{
	public class DealerDashboardViewModel
	{
		public DealerDto Dealer { get; set; } = new();

		public List<CarDto> ActiveListings { get; set; } = new();

		public List<CarDto> SoldCars { get; set; } = new();

		public List<DealerSaleViewModel> RecentSales { get; set; } = new();

		public decimal TotalRevenue { get; set; }

		public int ActiveListingsCount { get; set; }

		public int SoldCarsCount { get; set; }

		public int FavoritesReceived { get; set; }
	}
}
