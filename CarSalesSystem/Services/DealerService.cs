using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
	public class DealerService : IDealerService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<DealerService> _logger;
		private readonly IFavoriteService _favoriteService;

		public DealerService(ApplicationDbContext context, IMapper mapper, ILogger<DealerService> logger, IFavoriteService favoriteService)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
			_favoriteService = favoriteService;
		}

		public void Add(DealerDto dto)
		{
			if (dto == null)
			{
				throw new ArgumentNullException(nameof(dto));
			}

			var dealer = new Dealer
			{
				Name = dto.Name,
				CompanyName = dto.CompanyName,
				PhoneNumber = dto.PhoneNumber,
				UserId = dto.UserId!
			};

			_context.Dealers.Add(dealer);
			_context.SaveChanges();
		}

		public bool CheckIsDealerByUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return false;
			}

			return _context.Dealers.Any(d => d.UserId == userId);
		}

		public Dealer? GetById(int id)
		{
			return _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == id);
		}

		public List<DealerDto> GetAll()
		{
			return _context.Dealers
				.Include(d => d.Cars)
				.Select(d => new DealerDto
				{
					Id = d.Id,
					Name = d.Name,
					CompanyName = d.CompanyName,
					PhoneNumber = d.PhoneNumber,
					UserId = d.UserId,
					ActiveListingsCount = d.Cars.Count(c => c.IsListed && !c.IsBought),
					SoldCarsCount = d.Cars.Count(c => c.IsBought)
				})
				.OrderByDescending(d => d.ActiveListingsCount)
				.ToList();
		}

		public void Update(Dealer dealer)
		{
			if (dealer == null)
			{
				throw new ArgumentNullException(nameof(dealer));
			}

			_context.Dealers.Update(dealer);
			_context.SaveChanges();
		}

		public Dealer Details(int id)
		{
			var dealer = _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == id);

			if (dealer == null)
			{
				throw new ArgumentException("Dealer not found!");
			}

			return dealer;
		}

		public void Edit(DealerDto dto)
		{
			try
			{
				var dealer = _context.Dealers.FirstOrDefault(d => d.Id == dto.Id);

				if (dealer == null)
				{
					throw new ArgumentException("Dealer not found!");
				}

				dealer.Name = dto.Name;
				dealer.CompanyName = dto.CompanyName;
				dealer.PhoneNumber = dto.PhoneNumber;

				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error editing dealer.");
				throw;
			}
		}

		public Dealer? GetDealerByUserId(string userId)
		{
			return _context.Dealers.FirstOrDefault(d => d.UserId == userId);
		}

		public void Delete(int id)
		{
			var dealer = _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == id);

			if (dealer == null)
			{
				throw new ArgumentException("Dealer not found.");
			}

			_context.Dealers.Remove(dealer);
			_context.SaveChanges();
		}

		public List<Car> GetAllCars(int dealerId)
		{
			var dealer = _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == dealerId);

			if (dealer == null)
			{
				throw new ArgumentException("Dealer not found.");
			}

			return dealer.Cars.ToList();
		}

		public DealerDashboardViewModel GetDashboard(string userId)
		{
			var dealer = GetDealerByUserId(userId);
			if (dealer == null)
			{
				throw new UnauthorizedAccessException("Dealer profile not found.");
			}

			var dealerCars = _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.Where(c => c.DealerId == dealer.Id)
				.OrderByDescending(c => c.Id)
				.ToList();

			var payments = _context.Payments
				.Include(p => p.Car)
				.Where(p => p.Car.DealerId == dealer.Id)
				.OrderByDescending(p => p.PaymentTime)
				.ToList();

			return new DealerDashboardViewModel
			{
				Dealer = new DealerDto
				{
					Id = dealer.Id,
					Name = dealer.Name,
					CompanyName = dealer.CompanyName,
					PhoneNumber = dealer.PhoneNumber,
					UserId = dealer.UserId
				},
				ActiveListings = _mapper.Map<List<CarDto>>(dealerCars.Where(c => c.IsListed && !c.IsBought).ToList()),
				SoldCars = _mapper.Map<List<CarDto>>(dealerCars.Where(c => c.IsBought).ToList()),
				RecentSales = payments.Take(8).Select(p => new DealerSaleViewModel
				{
					PaymentId = p.Id,
					CarId = p.CarId,
					CarTitle = $"{p.Car.Brand} {p.Car.Model}",
					Amount = p.TotalAmount,
					PaidOnUtc = p.PaymentTime,
					BuyerId = p.UserId ?? "anonymous"
				}).ToList(),
				TotalRevenue = payments.Sum(p => p.TotalAmount),
				ActiveListingsCount = dealerCars.Count(c => c.IsListed && !c.IsBought),
				SoldCarsCount = dealerCars.Count(c => c.IsBought),
				FavoritesReceived = _favoriteService.CountFavoritesForDealer(dealer.Id)
			};
		}

		public DealerPublicProfileViewModel? GetPublicProfile(int dealerId)
		{
			var dealer = _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == dealerId);

			if (dealer == null)
			{
				return null;
			}

			var activeCars = _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.Where(c => c.DealerId == dealerId && c.IsListed && !c.IsBought)
				.OrderByDescending(c => c.Id)
				.ToList();

			var totalRevenue = _context.Payments
				.Where(p => p.Car.DealerId == dealerId)
				.Select(p => p.TotalAmount)
				.ToList()
				.Sum();

			return new DealerPublicProfileViewModel
			{
				Dealer = new DealerDto
				{
					Id = dealer.Id,
					Name = dealer.Name,
					CompanyName = dealer.CompanyName,
					PhoneNumber = dealer.PhoneNumber,
					UserId = dealer.UserId
				},
				ActiveListings = _mapper.Map<List<CarDto>>(activeCars),
				SoldCarsCount = _context.Cars.Count(c => c.DealerId == dealerId && c.IsBought),
				TotalRevenue = totalRevenue
			};
		}

		public void ToggleListingStatus(string userId, int carId)
		{
			var dealer = GetDealerByUserId(userId);
			if (dealer == null)
			{
				throw new UnauthorizedAccessException("Dealer profile not found.");
			}

			var car = _context.Cars.FirstOrDefault(c => c.Id == carId);
			if (car == null)
			{
				throw new ArgumentException("Car not found.");
			}

			if (car.DealerId != dealer.Id)
			{
				throw new UnauthorizedAccessException("You can only manage your own listings.");
			}

			if (!car.IsBought)
			{
				car.IsListed = !car.IsListed;
				_context.SaveChanges();
			}
		}
	}
}
