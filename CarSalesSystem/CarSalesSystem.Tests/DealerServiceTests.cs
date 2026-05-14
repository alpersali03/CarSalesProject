using CarSalesSystem.Data.Model;
using CarSalesSystem.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CarSalesSystem.Tests;

public class DealerServiceTests
{
	[Fact]
	public void GetDashboard_ReturnsListingSalesRevenueAndFavoritesForDealer()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var favoriteService = new FavoriteService(context, mapper);
		var service = new DealerService(context, mapper, NullLogger<DealerService>.Instance, favoriteService);

		var dealer = TestInfrastructure.SeedDealer(context, "dealer-user", "Dealer One");
		var otherDealer = TestInfrastructure.SeedDealer(context, "other-dealer", "Dealer Two");
		var category = TestInfrastructure.SeedCategory(context, "SUV");

		var activeCar = TestInfrastructure.SeedCar(context, dealer, category, brand: "BMW", model: "X5", isListed: true, isBought: false, price: 64000m);
		var soldCar = TestInfrastructure.SeedCar(context, dealer, category, brand: "Audi", model: "Q7", isListed: true, isBought: true, price: 72000m);
		var otherCar = TestInfrastructure.SeedCar(context, otherDealer, category, brand: "VW", model: "Touareg", isListed: true, isBought: false, price: 40000m);

		context.Payments.AddRange(
			new Payment
			{
				CarId = soldCar.Id,
				UserId = "buyer-1",
				PaymentTime = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc),
				TotalAmount = 72000m,
				IsSuccessful = true,
				CardLast4 = "1234",
				CardholderName = "Buyer One",
				ExpirationMonth = "12",
				ExpirationYear = 2030
			},
			new Payment
			{
				CarId = otherCar.Id,
				UserId = "buyer-2",
				PaymentTime = new DateTime(2026, 5, 2, 12, 0, 0, DateTimeKind.Utc),
				TotalAmount = 40000m,
				IsSuccessful = true,
				CardLast4 = "5678",
				CardholderName = "Buyer Two",
				ExpirationMonth = "11",
				ExpirationYear = 2031
			});

		context.FavoriteCars.Add(new FavoriteCar
		{
			UserId = "fan-1",
			CarId = activeCar.Id,
			CreatedOnUtc = DateTime.UtcNow
		});
		context.SaveChanges();

		var dashboard = service.GetDashboard("dealer-user");

		Assert.Equal(1, dashboard.ActiveListingsCount);
		Assert.Equal(1, dashboard.SoldCarsCount);
		Assert.Equal(64000m, dashboard.ActiveListings[0].Price);
		Assert.Single(dashboard.RecentSales);
		Assert.Equal(72000m, dashboard.TotalRevenue);
		Assert.Equal(1, dashboard.FavoritesReceived);
		Assert.Equal("Audi Q7", dashboard.RecentSales[0].CarTitle);
	}

	[Fact]
	public void GetPublicProfile_ReturnsOnlyActiveCarsAndDealerRevenue()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var favoriteService = new FavoriteService(context, mapper);
		var service = new DealerService(context, mapper, NullLogger<DealerService>.Instance, favoriteService);

		var dealer = TestInfrastructure.SeedDealer(context, "dealer-user", "Dealer One");
		var category = TestInfrastructure.SeedCategory(context, "Sedan");

		var activeCar = TestInfrastructure.SeedCar(context, dealer, category, brand: "Tesla", model: "Model 3", isListed: true, isBought: false, price: 62000m);
		_ = TestInfrastructure.SeedCar(context, dealer, category, brand: "BMW", model: "530d", isListed: true, isBought: true, price: 38000m);
		_ = TestInfrastructure.SeedCar(context, dealer, category, brand: "Audi", model: "A6", isListed: false, isBought: false, price: 30000m);

		context.Payments.Add(new Payment
		{
			CarId = activeCar.Id,
			UserId = "buyer-1",
			PaymentTime = DateTime.UtcNow,
			TotalAmount = 62000m,
			IsSuccessful = true,
			CardLast4 = "1111",
			CardholderName = "Buyer",
			ExpirationMonth = "10",
			ExpirationYear = 2030
		});
		context.SaveChanges();

		var profile = service.GetPublicProfile(dealer.Id);

		Assert.NotNull(profile);
		Assert.Single(profile!.ActiveListings);
		Assert.Equal("Tesla", profile.ActiveListings[0].Brand);
		Assert.Equal(1, profile.SoldCarsCount);
		Assert.Equal(62000m, profile.TotalRevenue);
	}

	[Fact]
	public void ToggleListingStatus_ThrowsForAnotherDealersCar()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var favoriteService = new FavoriteService(context, mapper);
		var service = new DealerService(context, mapper, NullLogger<DealerService>.Instance, favoriteService);

		var owner = TestInfrastructure.SeedDealer(context, "owner-user");
		var intruder = TestInfrastructure.SeedDealer(context, "intruder-user");
		var category = TestInfrastructure.SeedCategory(context);
		var car = TestInfrastructure.SeedCar(context, owner, category, isListed: true, isBought: false);

		var ex = Assert.Throws<UnauthorizedAccessException>(() => service.ToggleListingStatus(intruder.UserId, car.Id));

		Assert.Equal("You can only manage your own listings.", ex.Message);
		Assert.True(context.Cars.Single().IsListed);
	}
}
