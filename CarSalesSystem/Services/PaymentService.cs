using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly ApplicationDbContext _context;
		public PaymentService(ApplicationDbContext context)
		{
			_context = context;
		}
		public void Add(PaymentDto dto)
		{
			var payment = new Payment
			{
				PaymentTime = dto.PaymentTime,
				TotalAmount = dto.TotalAmount,
				IsSuccessful = dto.IsSuccessful,
			};
			_context.Payments.Add(payment);
			_context.SaveChanges();
		}

		public void Buy(PaymentDto dto, string userId)
		{
			var debitCard = new DebitCard
			{
				CardNumber = dto.CardNumber,
				CVV = dto.CVV,
				FullName = dto.FullName,
				ExpirationMonth = dto.ExpirationMonth,
				ExpirationYear = dto.ExpirationYear,

			};
			_context.DebitCards.Add(debitCard);
			_context.SaveChanges();

			// 2. Look up car price
			var car = _context.Cars.FirstOrDefault(c => c.Id == dto.CarId);

			// 3. Save the payment
			var payment = new Payment
			{
				PaymentTime = DateTime.Now,
				TotalAmount = car?.Price ?? 0,
				IsSuccessful = true,
				DebitCardId = debitCard.Id,
				CarId = dto.CarId,
				UserId = userId
			};
			_context.Payments.Add(payment);
			_context.SaveChanges();

		}

		public void Detalis(int id)
		{
			throw new NotImplementedException();
		}
	}
}
