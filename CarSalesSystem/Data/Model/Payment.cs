using System.ComponentModel.DataAnnotations;
namespace CarSalesSystem.Data.Model
{
	public class Payment
	{
		[Key]
		public int Id { get; set; }
		public DateTime PaymentTime { get; set; }
		[Required]
		public decimal TotalAmount { get; set; }
		public string? UserId { get; set; }  
		[Required]
		public int CarId { get; set; }
		public Car Car { get; set; } = null!;
		[Required]
		[StringLength(4)]
		public string CardLast4 { get; set; } = null!;
		[Required]
		public string CardholderName { get; set; } = null!;
		[Required]
		[StringLength(2)]
		public string ExpirationMonth { get; set; } = null!;
		[Required]
		public int ExpirationYear { get; set; }
		public bool IsSuccessful { get; set; }

	}
}
