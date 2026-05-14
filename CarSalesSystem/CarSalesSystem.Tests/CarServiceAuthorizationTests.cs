using CarSalesSystem.DTOs;
using CarSalesSystem.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CarSalesSystem.Tests;

public class CarServiceAuthorizationTests
{
	[Fact]
	public void Edit_ThrowsWhenDealerDoesNotOwnCar()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var service = new CarService(
			context,
			mapper,
			NullLogger<CarService>.Instance,
			new TestInfrastructure.StubDealerService(context));

		var owner = TestInfrastructure.SeedDealer(context, "owner-user");
		var otherDealer = TestInfrastructure.SeedDealer(context, "other-user");
		var category = TestInfrastructure.SeedCategory(context);
		var car = TestInfrastructure.SeedCar(context, owner, category, brand: "Audi", model: "A6");

		var dto = new CarFormDto
		{
			Brand = "BMW",
			Model = "M5",
			Description = "Updated",
			ImageUrl = "https://example.com/updated.jpg",
			Year = 2024,
			Mileage = 1000,
			FuelType = "Petrol",
			Transmission = "Automatic",
			Price = 100000m,
			Country = "Bulgaria",
			City = "Sofia",
			CategoryId = category.Id,
			IsListed = true
		};

		var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Edit(car.Id, dto, otherDealer.UserId));

		Assert.Equal("You can only edit your own cars.", ex.Message);
		Assert.Equal("Audi", context.Cars.Single().Brand);
	}

	[Fact]
	public void Delete_ThrowsWhenUserIsNotDealer()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var service = new CarService(
			context,
			mapper,
			NullLogger<CarService>.Instance,
			new TestInfrastructure.StubDealerService(context));

		var owner = TestInfrastructure.SeedDealer(context, "owner-user");
		var category = TestInfrastructure.SeedCategory(context);
		var car = TestInfrastructure.SeedCar(context, owner, category);

		var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Delete("plain-user", car.Id));

		Assert.Equal("Only dealers can delete cars.", ex.Message);
		Assert.Single(context.Cars);
	}

	[Fact]
	public void Add_AssignsDealerFromUserAndForcesListedState()
	{
		using var context = TestInfrastructure.CreateContext();
		var mapper = TestInfrastructure.CreateMapper();
		var dealer = TestInfrastructure.SeedDealer(context, "dealer-user");
		var category = TestInfrastructure.SeedCategory(context);
		var service = new CarService(
			context,
			mapper,
			NullLogger<CarService>.Instance,
			new TestInfrastructure.StubDealerService(context));

		var dto = new CarFormDto
		{
			Brand = "Tesla",
			Model = "Model Y",
			Description = "Fresh listing",
			ImageUrl = "https://example.com/modely.jpg",
			Year = 2023,
			Mileage = 5000,
			FuelType = "Electric",
			Transmission = "Automatic",
			Price = 72000m,
			Country = "Bulgaria",
			City = "Varna",
			CategoryId = category.Id,
			UserId = dealer.UserId,
			IsListed = false
		};

		service.Add(dto);

		var savedCar = context.Cars.Single(c => c.Brand == "Tesla");
		Assert.Equal(dealer.Id, savedCar.DealerId);
		Assert.True(savedCar.IsListed);
	}
}
