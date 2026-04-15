using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
	public class FavoriteService : IFavoriteService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public FavoriteService(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public void ToggleFavorite(string userId, int carId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new UnauthorizedAccessException("You must be signed in to manage favorites.");
			}

			var existingFavorite = _context.FavoriteCars
				.FirstOrDefault(f => f.UserId == userId && f.CarId == carId);

			if (existingFavorite != null)
			{
				_context.FavoriteCars.Remove(existingFavorite);
			}
			else
			{
				var carExists = _context.Cars.Any(c => c.Id == carId && !c.IsBought);
				if (!carExists)
				{
					throw new ArgumentException("Car not found.");
				}

				_context.FavoriteCars.Add(new FavoriteCar
				{
					UserId = userId,
					CarId = carId,
					CreatedOnUtc = DateTime.UtcNow
				});
			}

			_context.SaveChanges();
		}

		public HashSet<int> GetFavoriteCarIds(string? userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return [];
			}

			return _context.FavoriteCars
				.Where(f => f.UserId == userId)
				.Select(f => f.CarId)
				.ToHashSet();
		}

		public List<CarDto> GetFavoriteCars(string userId)
		{
			var favoriteCarIds = _context.FavoriteCars
				.Where(f => f.UserId == userId)
				.Select(f => f.CarId);

			var cars = _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.Where(c => favoriteCarIds.Contains(c.Id))
				.Where(c => !c.IsBought)
				.OrderByDescending(c => c.Id)
				.ToList();

			return _mapper.Map<List<CarDto>>(cars);
		}

		public int CountFavoritesForDealer(int dealerId)
		{
			return _context.FavoriteCars.Count(f => f.Car.DealerId == dealerId);
		}
	}
}
