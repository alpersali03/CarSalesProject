using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
	// Legacy type retained only so historical EF migrations continue to compile.
	// The current application no longer maps or persists raw debit card data.
	public class DebitCard
	{
		[Key]
		public int Id { get; set; }

		public string CardNumber { get; set; } = null!;

		public int CVV { get; set; }

		public string FullName { get; set; } = null!;

		public string ExpirationMonth { get; set; } = null!;

		public int ExpirationYear { get; set; }

		public List<Payment> Payments { get; set; } = new();
	}
}
