namespace CarSalesSystem.DTOs
{
	public class PaymentDto
	{
		public DateTime PaymentTime { get; set; }
		public decimal TotalAmount { get; set; }
		public bool IsSuccessful { get; set; }

		public int CarId { get; set; }

		public string CardNumber { get; set; } = null!;
		public int CVV { get; set; }
		public string FullName { get; set; } = null!;
		public string ExpirationMonth { get; set; } = null!;
		public int ExpirationYear { get; set; }
	}
}
