using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Models;
using CarSalesSystem.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CarSalesSystem.Tests;

public class MarketplaceFeatureTests
{
	[Fact]
	public void ToggleFavorite_AddsAndRemovesFavoriteForSignedInUser()
	{
		using var context = CreateContext();
		var mapper = CreateMapper();
		var car = SeedCar(context, "Audi", "A4", "Petrol", "Sofia", 2022, 42000m, isListed: true, isBought: false);
		var service = new FavoriteService(context, mapper);

		service.ToggleFavorite("buyer-1", car.Id);

		var favorites = service.GetFavoriteCars("buyer-1");
		Assert.Single(favorites);
		Assert.Equal(car.Id, favorites.Single().Id);

		service.ToggleFavorite("buyer-1", car.Id);

		Assert.Empty(service.GetFavoriteCars("buyer-1"));
	}

	[Fact]
	public void Search_AppliesBrandCityAndPriceFilters()
	{
		using var context = CreateContext();
		var mapper = CreateMapper();
		var dealer = SeedDealer(context, "dealer-user");
		var category = SeedCategory(context, "SUV");

		context.Cars.AddRange(
			new Car
			{
				Brand = "BMW",
				Model = "X5",
				Description = "Premium SUV",
				ImageUrl = "https://example.com/x5.jpg",
				Year = 2021,
				Mileage = 55000,
				FuelType = "Diesel",
				Transmission = "Automatic",
				Price = 64000m,
				IsListed = true,
				IsBought = false,
				Country = "Bulgaria",
				City = "Sofia",
				CategoryId = category.Id,
				DealerId = dealer.Id
			},
			new Car
			{
				Brand = "BMW",
				Model = "320",
				Description = "Sedan",
				ImageUrl = "https://example.com/320.jpg",
				Year = 2018,
				Mileage = 110000,
				FuelType = "Diesel",
				Transmission = "Automatic",
				Price = 26000m,
				IsListed = true,
				IsBought = false,
				Country = "Bulgaria",
				City = "Plovdiv",
				CategoryId = category.Id,
				DealerId = dealer.Id
			},
			new Car
			{
				Brand = "Audi",
				Model = "Q5",
				Description = "Listed elsewhere",
				ImageUrl = "https://example.com/q5.jpg",
				Year = 2022,
				Mileage = 44000,
				FuelType = "Diesel",
				Transmission = "Automatic",
				Price = 70000m,
				IsListed = false,
				IsBought = false,
				Country = "Bulgaria",
				City = "Sofia",
				CategoryId = category.Id,
				DealerId = dealer.Id
			});
		context.SaveChanges();

		var service = new CarService(
			context,
			mapper,
			NullLogger<CarService>.Instance,
			new StubDealerService(context));

		var results = service.Search(new CarSearchViewModel
		{
			BrandType = "BMW",
			City = "Sofia",
			MinPrice = 50000m,
			MaxPrice = 70000m
		});

		Assert.Single(results);
		Assert.Equal("BMW", results[0].Brand);
		Assert.Equal("X5", results[0].Model);
	}

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

	private static IMapper CreateMapper()
	{
		var config = new MapperConfiguration(
			cfg => cfg.AddProfile<MappingProfile>(),
			NullLoggerFactory.Instance);
		return config.CreateMapper();
	}

	private static Car SeedCar(
		ApplicationDbContext context,
		string brand,
		string model,
		string fuelType,
		string city,
		int year,
		decimal price,
		bool isListed,
		bool isBought)
	{
		var dealer = SeedDealer(context, Guid.NewGuid().ToString());
		var category = SeedCategory(context, $"Category-{Guid.NewGuid()}");

		var car = new Car
		{
			Brand = brand,
			Model = model,
			Description = $"{brand} {model}",
			ImageUrl = "https://example.com/car.jpg",
			Year = year,
			Mileage = 10000,
			FuelType = fuelType,
			Transmission = "Automatic",
			Price = price,
			IsListed = isListed,
			IsBought = isBought,
			Country = "Bulgaria",
			City = city,
			CategoryId = category.Id,
			DealerId = dealer.Id
		};

		context.Cars.Add(car);
		context.SaveChanges();
		return car;
	}

	private static Dealer SeedDealer(ApplicationDbContext context, string userId)
	{
		var dealer = new Dealer
		{
			Name = $"Dealer-{Guid.NewGuid()}",
			CompanyName = "Auto House",
			PhoneNumber = "0888123456",
			UserId = userId
		};

		context.Dealers.Add(dealer);
		context.SaveChanges();
		return dealer;
	}

	private static Category SeedCategory(ApplicationDbContext context, string name)
	{
		var category = new Category { Name = name };
		context.Categories.Add(category);
		context.SaveChanges();
		return category;
	}

	private sealed class StubDealerService : IDealerService
	{
		private readonly ApplicationDbContext _context;

		public StubDealerService(ApplicationDbContext context)
		{
			_context = context;
		}

		public void Add(DealerDto dto) => throw new NotSupportedException();
		public bool CheckIsDealerByUserId(string userId) => _context.Dealers.Any(d => d.UserId == userId);
		public Dealer? GetById(int id) => _context.Dealers.FirstOrDefault(d => d.Id == id);
		public List<DealerDto> GetAll() => throw new NotSupportedException();
		public void Update(Dealer dealer) => throw new NotSupportedException();
		public Dealer Details(int id) => throw new NotSupportedException();
		public void Edit(DealerDto dto) => throw new NotSupportedException();
		public Dealer? GetDealerByUserId(string userId) => _context.Dealers.FirstOrDefault(d => d.UserId == userId);
		public void Delete(int id) => throw new NotSupportedException();
		public List<Car> GetAllCars(int dealerId) => throw new NotSupportedException();
		public DealerDashboardViewModel GetDashboard(string userId) => throw new NotSupportedException();
		public DealerPublicProfileViewModel? GetPublicProfile(int dealerId) => throw new NotSupportedException();
		public void ToggleListingStatus(string userId, int carId) => throw new NotSupportedException();
	}
}
