using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IPaymentService
	{
		List<Payment> GetPaymentsForUser(string userId, bool isManager);

		Payment? GetPaymentForUser(int id, string userId, bool isManager);

		PaymentPurchaseResult Buy(PaymentDto dto, string userId);
	}
}
