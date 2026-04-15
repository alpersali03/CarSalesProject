namespace CarSalesSystem.Models
{
	public class DealerSaleViewModel
	{
		public int PaymentId { get; set; }

		public int CarId { get; set; }

		public string CarTitle { get; set; } = null!;

		public decimal Amount { get; set; }

		public DateTime PaidOnUtc { get; set; }

		public string BuyerId { get; set; } = null!;
	}
}
