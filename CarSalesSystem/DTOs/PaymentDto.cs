using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
	public class PaymentDto
	{
		[Required]
		public int CarId { get; set; }

		[Required]
		[CreditCard]
		public string CardNumber { get; set; } = null!;

		[Required]
		[RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
		public string CVV { get; set; } = null!;

		[Required]
		public string FullName { get; set; } = null!;

		[Required]
		[RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Expiration month must be in MM format.")]
		public string ExpirationMonth { get; set; } = null!;

		[Range(2024, 2100)]
		public int ExpirationYear { get; set; }
	}
}
