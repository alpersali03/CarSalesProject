using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
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

		public List<Payment> GetPaymentsForUser(string userId, bool isManager)
		{
			var query = _context.Payments
				.Include(p => p.Car)
				.AsQueryable();

			if (!isManager)
			{
				query = query.Where(p => p.UserId == userId);
			}

			return query
				.OrderByDescending(p => p.PaymentTime)
				.ToList();
		}

		public Payment? GetPaymentForUser(int id, string userId, bool isManager)
		{
			var query = _context.Payments
				.Include(p => p.Car)
				.Where(p => p.Id == id);

			if (!isManager)
			{
				query = query.Where(p => p.UserId == userId);
			}

			return query.FirstOrDefault();
		}

		public PaymentPurchaseResult Buy(PaymentDto dto, string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return PaymentPurchaseResult.Failure("You must be signed in to buy a car.");
			}

			using var transaction = _context.Database.BeginTransaction();

			var car = _context.Cars.FirstOrDefault(c => c.Id == dto.CarId);
			if (car == null)
			{
				return PaymentPurchaseResult.Failure("The selected car was not found.");
			}

			if (car.IsBought)
			{
				return PaymentPurchaseResult.Failure("This car has already been sold.");
			}

			var normalizedCardNumber = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
			if (normalizedCardNumber.Length < 4)
			{
				return PaymentPurchaseResult.Failure("The card number is invalid.");
			}

			var payment = new Payment
			{
				PaymentTime = DateTime.UtcNow,
				TotalAmount = car.Price,
				IsSuccessful = true,
				CarId = dto.CarId,
				UserId = userId,
				CardLast4 = normalizedCardNumber[^4..],
				CardholderName = dto.FullName.Trim(),
				ExpirationMonth = dto.ExpirationMonth,
				ExpirationYear = dto.ExpirationYear
			};

			_context.Payments.Add(payment);
			car.IsBought = true;
			_context.SaveChanges();
			transaction.Commit();

			return PaymentPurchaseResult.Success();
		}
	}
}
