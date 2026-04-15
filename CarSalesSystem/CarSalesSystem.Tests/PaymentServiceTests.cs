using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarSalesSystem.Tests;

public class PaymentServiceTests
{
	[Fact]
	public void Buy_ForUnsoldCar_CreatesPaymentAndMarksCarAsBought()
	{
		using var context = CreateContext();
		var car = SeedCar(context, isBought: false);
		var service = new PaymentService(context);

		var result = service.Buy(CreatePaymentDto(car.Id), "buyer-1");

		Assert.True(result.Succeeded);
		Assert.Single(context.Payments);

		var payment = context.Payments.Single();
		Assert.Equal("1111", payment.CardLast4);
		Assert.Equal("Test Buyer", payment.CardholderName);
		Assert.True(context.Cars.Single().IsBought);
	}

	[Fact]
	public void Buy_ForSoldCar_ReturnsFailureWithoutCreatingPayment()
	{
		using var context = CreateContext();
		var car = SeedCar(context, isBought: true);
		var service = new PaymentService(context);

		var result = service.Buy(CreatePaymentDto(car.Id), "buyer-1");

		Assert.False(result.Succeeded);
		Assert.Equal("This car has already been sold.", result.ErrorMessage);
		Assert.Empty(context.Payments);
		Assert.True(context.Cars.Single().IsBought);
	}

	[Fact]
	public void GetPaymentsForUser_FiltersPaymentsForNonManagers()
	{
		using var context = CreateContext();
		var firstCar = SeedCar(context, isBought: false, brand: "Audi", model: "A4");
		var secondCar = SeedCar(context, isBought: false, brand: "BMW", model: "320");

		context.Payments.AddRange(
			new Payment
			{
				CarId = firstCar.Id,
				UserId = "buyer-1",
				PaymentTime = DateTime.UtcNow,
				TotalAmount = firstCar.Price,
				IsSuccessful = true,
				CardLast4 = "1111",
				CardholderName = "Buyer One",
				ExpirationMonth = "12",
				ExpirationYear = 2030
			},
			new Payment
			{
				CarId = secondCar.Id,
				UserId = "buyer-2",
				PaymentTime = DateTime.UtcNow.AddMinutes(-5),
				TotalAmount = secondCar.Price,
				IsSuccessful = true,
				CardLast4 = "2222",
				CardholderName = "Buyer Two",
				ExpirationMonth = "11",
				ExpirationYear = 2031
			});
		context.SaveChanges();

		var service = new PaymentService(context);

		var userPayments = service.GetPaymentsForUser("buyer-1", isManager: false);
		var managerPayments = service.GetPaymentsForUser("manager-1", isManager: true);

		Assert.Single(userPayments);
		Assert.Equal("buyer-1", userPayments.Single().UserId);
		Assert.Equal(2, managerPayments.Count);
	}

	private static PaymentDto CreatePaymentDto(int carId) => new()
	{
		CarId = carId,
		CardNumber = "4111111111111111",
		CVV = "123",
		FullName = "Test Buyer",
		ExpirationMonth = "12",
		ExpirationYear = 2030
	};

	private static ApplicationDbContext CreateContext()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();

		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseSqlite(connection)
			.Options;

		var context = new ApplicationDbContext(options);
		context.Database.EnsureDeleted();
		context.Database.EnsureCreated();
		return context;
	}

	private static Car SeedCar(ApplicationDbContext context, bool isBought, string brand = "VW", string model = "Golf")
	{
		var category = new Category { Name = $"Category-{Guid.NewGuid()}" };
		var dealer = new Dealer
		{
			Name = $"Dealer-{Guid.NewGuid()}",
			PhoneNumber = "0888123456",
			UserId = Guid.NewGuid().ToString()
		};

		context.Categories.Add(category);
		context.Dealers.Add(dealer);
		context.SaveChanges();

		var car = new Car
		{
			Brand = brand,
			Model = model,
			Description = "Test car",
			ImageUrl = "https://example.com/car.jpg",
			Year = 2020,
			Mileage = 120000,
			FuelType = "Diesel",
			Transmission = "Manual",
			Price = 15000,
			IsListed = true,
			IsBought = isBought,
			Country = "Bulgaria",
			City = "Sofia",
			CategoryId = category.Id,
			DealerId = dealer.Id
		};

		context.Cars.Add(car);
		context.SaveChanges();
		return car;
	}
}
