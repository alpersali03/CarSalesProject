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

namespace CarSalesSystem.Tests;

internal static class TestInfrastructure
{
	public static ApplicationDbContext CreateContext()
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

	public static IMapper CreateMapper()
	{
		var config = new MapperConfiguration(
			cfg => cfg.AddProfile<MappingProfile>(),
			NullLoggerFactory.Instance);
		return config.CreateMapper();
	}

	public static Dealer SeedDealer(ApplicationDbContext context, string userId, string? name = null)
	{
		var dealer = new Dealer
		{
			Name = name ?? $"Dealer-{Guid.NewGuid()}",
			CompanyName = "Auto House",
			PhoneNumber = "0888123456",
			UserId = userId
		};

		context.Dealers.Add(dealer);
		context.SaveChanges();
		return dealer;
	}

	public static Category SeedCategory(ApplicationDbContext context, string? name = null)
	{
		var category = new Category { Name = name ?? $"Category-{Guid.NewGuid()}" };
		context.Categories.Add(category);
		context.SaveChanges();
		return category;
	}

	public static Car SeedCar(
		ApplicationDbContext context,
		Dealer dealer,
		Category category,
		string brand = "VW",
		string model = "Golf",
		bool isListed = true,
		bool isBought = false,
		decimal price = 15000m,
		string city = "Sofia")
	{
		var car = new Car
		{
			Brand = brand,
			Model = model,
			Description = $"{brand} {model}",
			ImageUrl = "https://example.com/car.jpg",
			Year = 2020,
			Mileage = 10000,
			FuelType = "Diesel",
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

	public sealed class StubDealerService : IDealerService
	{
		private readonly ApplicationDbContext context;

		public StubDealerService(ApplicationDbContext context)
		{
			this.context = context;
		}

		public void Add(DealerDto dto) => throw new NotSupportedException();
		public bool CheckIsDealerByUserId(string userId) => context.Dealers.Any(d => d.UserId == userId);
		public Dealer? GetById(int id) => context.Dealers.FirstOrDefault(d => d.Id == id);
		public List<DealerDto> GetAll() => throw new NotSupportedException();
		public void Update(Dealer dealer) => throw new NotSupportedException();
		public Dealer Details(int id) => throw new NotSupportedException();
		public void Edit(DealerDto dto) => throw new NotSupportedException();
		public Dealer? GetDealerByUserId(string userId) => context.Dealers.FirstOrDefault(d => d.UserId == userId);
		public void Delete(int id) => throw new NotSupportedException();
		public List<Car> GetAllCars(int dealerId) => throw new NotSupportedException();
		public DealerDashboardViewModel GetDashboard(string userId) => throw new NotSupportedException();
		public DealerPublicProfileViewModel? GetPublicProfile(int dealerId) => throw new NotSupportedException();
		public void ToggleListingStatus(string userId, int carId) => throw new NotSupportedException();
	}
}
