using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
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

			if (string.IsNullOrEmpty(dto.Brand) ||
		string.IsNullOrEmpty(dto.Model) ||
		string.IsNullOrEmpty(dto.City) ||
		string.IsNullOrEmpty(dto.Country) ||
		string.IsNullOrEmpty(dto.Transmission))
			{
				throw new ArgumentNullException("Cannot add a car, nullable data added!");
			}
			var car = _mapper.Map<Car>(dto);
				car.IsListed = true;
			
			car.DealerId = _dealerService.GetDealerByUserId(dto.UserId).Id;
			_context.Cars.Add(car);

				_context.SaveChanges();
			
			
		}


		public Car GetById(int id)
		{
			return _context.Cars.FirstOrDefault(c => c.Id == id);
		}


		public void Edit(int id, CarFormDto dto)
		{
			try
			{
				var car = _context.Cars.FirstOrDefault(c => c.Id == id);

				if (car == null)
					throw new ArgumentException("Car not found");

				car.Id = dto.Id;
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
				car.DealerId = dto.DealerId;

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

			var cars = this._context.Cars.Include(p => p.Category).Where(p => p.Model.Contains(keyword)).ToList();

			var mapped = _mapper.Map<List<CarDto>>(cars);

			return mapped;
		}

		public List<CarDto> SortByPrice(string sortOrder)
		{
			var cars = this._context.Cars.Include(p => p.Category).ToList();

			switch (sortOrder)
			{
				case "asc":

					cars = cars.OrderBy(p => p.Price).ToList();
					break;
				case "desc":

					cars = cars.OrderByDescending(p => p.Price).ToList();
					break;
				default:
					cars = cars.OrderBy(p => p.Id).ToList(); // default sort
					break;
			}
			var mapped = _mapper.Map<List<CarDto>>(cars);

			return mapped;
		}

		public List<CarDto> SortByName(string letter)
		{
			var cars = this._context.Cars.Include(p => p.Category).ToList();

			switch (letter)
			{
				case "asc":
					cars = cars.OrderBy(p => p.Model).ToList();
					break;
				case "desc":

					cars = cars.OrderByDescending(p => p.Model).ToList();
					break;
				default:
					cars = cars.OrderBy(p => p.Id).ToList(); // default sort
					break;
			}
			var mapped = _mapper.Map<List<CarDto>>(cars);



			return mapped;

		}

		public List<CarDto> GetLatest(int count)
		{
			var cars = _context.Cars
				.Where(c => c.IsListed)
				.OrderByDescending(c => c.Id)
				.Take(count)
				.ToList();

			return _mapper.Map<List<CarDto>>(cars);
		}

		public List<CarDto> GetByFuel(string fuelType)
		{
			var cars = _context.Cars
				.Where(c => c.FuelType == fuelType)
				.ToList();
			return _mapper.Map<List<CarDto>>(cars);
		}

		public List<CarDto> GetByYear(int? minYear, int? maxYear)
		{
			if (minYear.HasValue && maxYear.HasValue && minYear > maxYear)
			{
				(minYear, maxYear) = (maxYear, minYear);
			}

			var query = _context.Cars.AsQueryable();

			if (minYear.HasValue)
				query = query.Where(c => c.Year >= minYear.Value);

			if (maxYear.HasValue)
				query = query.Where(c => c.Year <= maxYear.Value);

			var cars = query
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.ToList();

			return _mapper.Map<List<CarDto>>(cars);
		}

		public List<CarDto> GetByBrand(string brandType)
		{
			var cars = _context.Cars
				.Where(c => c.Brand == brandType)
				.Include(c => c.Category)
				.Include(c => c.Dealer)
				.ToList();
			return _mapper.Map<List<CarDto>>(cars);
		}
		
	}
}
