using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Humanizer;

namespace CarSalesSystem.Services
{
	public class DealerService : IDealerService
	{
		
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		public DealerService(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public void Add(DealerDto dealer)
		{
			if (string.IsNullOrWhiteSpace(dealer.Name))
			{
				throw new ArgumentNullException("There is no such dealer!");
			}
			var getUserId = dealer.UserId;
			if (getUserId == null)
			{
				throw new ArgumentNullException("There is no dealer with this id!");
			}
			var delaer = new Dealer
			{
				Name = dealer.Name,
				CompanyName = dealer.CompanyName,
				PhoneNumber = dealer.PhoneNumber,
				UserId = getUserId,
			};
			_context.Dealers.Add(delaer);
			_context.SaveChanges();
			
		}

		public bool CheckIsDealerByUserId(string userId)
		{
			var getUserId = _context.Users.FirstOrDefault(x=>x.Id == userId);	
			if (getUserId == null)
			{
				return false;
			}
			var dealer = _context.Dealers.FirstOrDefault(x=>x.UserId == userId);

            if (dealer == null)
			{
				return false;

			}
			return true;


        }

		public void Edit(DealerDto dealer)
		{
			var dealerDto = _context.Dealers.FirstOrDefault(d => d.Id == dealer.Id);
			if (dealer == null)
			{
				throw new ArgumentException("Dealer not found!");
			}
			var dto = new DealerDto
			{
				Id = dealer.Id,
				Name = dealer.Name,
				CompanyName = dealer.CompanyName,
				PhoneNumber = dealer.PhoneNumber,
				UserId = dealer.UserId,
			};
		}

		public List<DealerDto> GetAll()
		{
			var dealers = _context.Dealers.ToList();
			return _mapper.Map<List<DealerDto>>(dealers);
		}
	}
}
