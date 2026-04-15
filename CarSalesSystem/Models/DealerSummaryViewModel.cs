namespace CarSalesSystem.Models
{
	public class DealerSummaryViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string? CompanyName { get; set; }

		public string PhoneNumber { get; set; } = null!;

		public int ActiveListingsCount { get; set; }

		public int SoldCarsCount { get; set; }

		public decimal Revenue { get; set; }
	}
}
