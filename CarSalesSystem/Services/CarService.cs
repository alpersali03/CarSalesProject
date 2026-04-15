using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
	public class CarService : ICarService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<CarService> _logger;
		private readonly IDealerService _dealerService;

		public CarService(ApplicationDbContext context, IMapper mapper, ILogger<CarService> logger, IDealerService dealerService)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
			_dealerService = dealerService;
		}

		public void Add(CarFormDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.UserId))
			{
				throw new UnauthorizedAccessException("Only authenticated dealers can add cars.");
			}

			var dealer = _dealerService.GetDealerByUserId(dto.UserId);
			if (dealer == null)
			{
				throw new UnauthorizedAccessException("Only registered dealers can add cars.");
			}

			var car = _mapper.Map<Car>(dto);
			car.IsListed = true;
			car.DealerId = dealer.Id;
			_context.Cars.Add(car);
			_context.SaveChanges();
		}

		public Car? GetById(int id)
		{
			return _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.FirstOrDefault(c => c.Id == id);
		}

		public List<Car> GetByIds(IEnumerable<int> ids)
		{
			var normalizedIds = ids.Distinct().Take(4).ToList();
			return _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.Where(c => normalizedIds.Contains(c.Id))
				.ToList();
		}

		public void Edit(int id, CarFormDto dto, string userId)
		{
			try
			{
				var dealer = _dealerService.GetDealerByUserId(userId);
				if (dealer == null)
				{
					throw new UnauthorizedAccessException("Only registered dealers can edit cars.");
				}

				var car = _context.Cars.FirstOrDefault(c => c.Id == id);
				if (car == null)
				{
					throw new ArgumentException("Car not found.");
				}

				if (car.DealerId != dealer.Id)
				{
					throw new UnauthorizedAccessException("You can only edit your own cars.");
				}

				car.Brand = dto.Brand;
				car.Model = dto.Model;
				car.Description = dto.Description;
				car.ImageUrl = dto.ImageUrl;
				car.Year = dto.Year;
				car.Mileage = dto.Mileage;
				car.FuelType = dto.FuelType;
				car.Transmission = dto.Transmission;
				car.Price = dto.Price;
				car.Country = dto.Country;
				car.City = dto.City;
				car.CategoryId = dto.CategoryId;
				car.IsListed = dto.IsListed;

				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to edit car.");
				throw;
			}
		}

		public List<CarDto> Search(string keyword)
		{
			if (string.IsNullOrEmpty(keyword))
			{
				throw new ArgumentException("Please add new keyword for searching");
			}

			var cars = BaseQuery()
				.Where(p => p.Model.Contains(keyword) || p.Brand.Contains(keyword))
				.ToList();
			return _mapper.Map<List<CarDto>>(cars);
		}

		public List<CarDto> SortByPrice(string sortOrder)
		{
			var query = BaseQuery();
			query = sortOrder switch
			{
				"asc" => query.OrderBy(p => p.Price),
				"desc" => query.OrderByDescending(p => p.Price),
				_ => query.OrderByDescending(p => p.Id)
			};

			return _mapper.Map<List<CarDto>>(query.ToList());
		}

		public List<CarDto> SortByName(string letter)
		{
			var query = BaseQuery();
			query = letter switch
			{
				"asc" => query.OrderBy(p => p.Model),
				"desc" => query.OrderByDescending(p => p.Model),
				_ => query.OrderByDescending(p => p.Id)
			};

			return _mapper.Map<List<CarDto>>(query.ToList());
		}

		public List<CarDto> GetLatest(int count)
		{
			var cars = BaseQuery()
				.OrderByDescending(c => c.Id)
				.Take(count)
				.ToList();

			return _mapper.Map<List<CarDto>>(cars);
		}

		public List<CarDto> GetByFuel(string fuelType)
		{
			return _mapper.Map<List<CarDto>>(
				BaseQuery().Where(c => c.FuelType == fuelType).ToList());
		}

		public List<CarDto> GetByYear(int? minYear, int? maxYear)
		{
			if (minYear.HasValue && maxYear.HasValue && minYear > maxYear)
			{
				(minYear, maxYear) = (maxYear, minYear);
			}

			var query = BaseQuery();

			if (minYear.HasValue)
			{
				query = query.Where(c => c.Year >= minYear.Value);
			}

			if (maxYear.HasValue)
			{
				query = query.Where(c => c.Year <= maxYear.Value);
			}

			return _mapper.Map<List<CarDto>>(query.ToList());
		}

		public List<CarDto> GetByBrand(string brandType)
		{
			return _mapper.Map<List<CarDto>>(
				BaseQuery().Where(c => c.Brand == brandType).ToList());
		}

		public List<CarDto> Search(CarSearchViewModel search)
		{
			var query = BaseQuery();

			if (!string.IsNullOrWhiteSpace(search.FuelType))
			{
				query = query.Where(c => c.FuelType == search.FuelType);
			}

			if (!string.IsNullOrWhiteSpace(search.BrandType))
			{
				query = query.Where(c => c.Brand == search.BrandType);
			}

			if (!string.IsNullOrWhiteSpace(search.City))
			{
				query = query.Where(c => c.City.Contains(search.City));
			}

			if (search.CategoryId.HasValue && search.CategoryId > 0)
			{
				query = query.Where(c => c.CategoryId == search.CategoryId.Value);
			}

			if (search.MinYear.HasValue)
			{
				query = query.Where(c => c.Year >= search.MinYear.Value);
			}

			if (search.MaxYear.HasValue)
			{
				query = query.Where(c => c.Year <= search.MaxYear.Value);
			}

			if (search.MinPrice.HasValue)
			{
				query = query.Where(c => c.Price >= search.MinPrice.Value);
			}

			if (search.MaxPrice.HasValue)
			{
				query = query.Where(c => c.Price <= search.MaxPrice.Value);
			}

			return _mapper.Map<List<CarDto>>(
				query.OrderByDescending(c => c.Id).ToList());
		}

		public void Delete(string userId, int id)
		{
			if (!_dealerService.CheckIsDealerByUserId(userId))
			{
				throw new UnauthorizedAccessException("Only dealers can delete cars.");
			}

			var car = _context.Cars.Find(id);
			if (car == null)
			{
				throw new ArgumentException("Car not found");
			}

			var dealer = _dealerService.GetDealerByUserId(userId);
			if (dealer == null || car.DealerId != dealer.Id)
			{
				throw new UnauthorizedAccessException("You can only delete your own cars.");
			}

			_context.Cars.Remove(car);
			_context.SaveChanges();
		}

		public List<string> PopulateBrands()
		{
			return BaseQuery()
				.Select(c => c.Brand)
				.Distinct()
				.OrderBy(b => b)
				.ToList();
		}

		public void Details(int id)
		{
			_ = GetById(id);
		}

		public List<CarDto> GetAll()
		{
			return _mapper.Map<List<CarDto>>(
				BaseQuery()
					.OrderByDescending(c => c.Id)
					.ToList());
		}

		private IQueryable<Car> BaseQuery()
		{
			return _context.Cars
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.Where(c => !c.IsBought && c.IsListed);
		}
	}
}
