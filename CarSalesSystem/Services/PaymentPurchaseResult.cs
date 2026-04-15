namespace CarSalesSystem.Services
{
	public class PaymentPurchaseResult
	{
		public bool Succeeded { get; init; }

		public string? ErrorMessage { get; init; }

		public static PaymentPurchaseResult Success() => new() { Succeeded = true };

		public static PaymentPurchaseResult Failure(string errorMessage) => new()
		{
			Succeeded = false,
			ErrorMessage = errorMessage
		};
	}
}
