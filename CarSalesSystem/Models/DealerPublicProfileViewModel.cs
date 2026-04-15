using CarSalesSystem.DTOs;

namespace CarSalesSystem.Models
{
	public class DealerPublicProfileViewModel
	{
		public DealerDto Dealer { get; set; } = new();

		public List<CarDto> ActiveListings { get; set; } = new();

		public int SoldCarsCount { get; set; }

		public decimal TotalRevenue { get; set; }
	}
}
