using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Services
{
	public class DealerService : IDealerService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<DealerService> _logger;

		public DealerService(ApplicationDbContext context, IMapper mapper, ILogger<DealerService> logger)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
		}

		public void Add(DealerDto dto)
		{
			try
			{
				if (dto == null)
					throw new ArgumentNullException(nameof(dto));

				var dealer = new Dealer
				{
					Name = dto.Name,
					CompanyName = dto.CompanyName,
					PhoneNumber = dto.PhoneNumber,
					UserId = dto.UserId
				};

				_context.Dealers.Add(dealer);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to add dealer.");
				throw;
			}
		}

		public bool CheckIsDealerByUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return false;

			return _context.Dealers.Any(d => d.UserId == userId);
		}

		public Dealer GetById(int id)
		{
			return _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == id);
		}

		public List<DealerDto> GetAll()
		{
			var dealers = _context.Dealers.ToList();
			return _mapper.Map<List<DealerDto>>(dealers);
		}

		public void Update(Dealer dealer)
		{
			try
			{
				if (dealer == null)
					throw new ArgumentNullException(nameof(dealer));

				_context.Dealers.Update(dealer);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to update dealer.");
				throw;
			}
		}

		public Dealer Details(int id)
		{
			var dealer = _context.Dealers
				.Include(d => d.Cars)
				.FirstOrDefault(d => d.Id == id);

			if (dealer == null)
				throw new ArgumentException("Dealer not found!");

			return dealer;
		}

		public void Edit(DealerDto dto)
		{
			try
			{
				var dealer = _context.Dealers.FirstOrDefault(d => d.Id == dto.Id);

				if (dealer == null)
					throw new ArgumentException("Dealer not found!");

				
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

	}
}
