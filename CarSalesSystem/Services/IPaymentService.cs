using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IPaymentService
	{
		void Add(PaymentDto dto);
		void Detalis(int id);
		void Buy(PaymentDto dto, string userId);

	}
}
