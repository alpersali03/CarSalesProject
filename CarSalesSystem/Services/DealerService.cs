using AutoMapper;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using CarSalesSystem.Extensions;
using System.Linq.Expressions;

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
			
			var delaer = new Dealer
			{
				Name = dealer.Name,
				CompanyName = dealer.CompanyName,
				PhoneNumber = dealer.PhoneNumber,
				UserId = dealer.UserId
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

		public void Details(int id)
		{
			var dealer = _context.Dealers.Include(d => d.Cars).FirstOrDefault(d => d.Id == id);
			if (dealer == null)
			{
				throw new ArgumentException("Dealer not found!");
			}
		}

		public void Edit(DealerDto dealer)
		{
			var dealers = _context.Dealers.FirstOrDefault(d => d.Id == dealer.Id);
			if (dealer == null)
			{
				throw new ArgumentNullException("There is no such dealer!");
			}
			
			dealer.Name = dealers.Name;
			dealer.CompanyName = dealers.CompanyName;
			dealer.PhoneNumber = dealers.PhoneNumber;

			_context.SaveChanges();
		}

		public List<DealerDto> GetAll()
		{
			var dealers = _context.Dealers.ToList();
			return _mapper.Map<List<DealerDto>>(dealers);
		}
	}
}
