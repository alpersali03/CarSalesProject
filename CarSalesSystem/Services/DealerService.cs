using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

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

		public List<DealerDto> GetAll()
		{
			var dealers = _context.Dealers.ToList();
			return _mapper.Map<List<DealerDto>>(dealers);
		}
	}
}
