using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;

namespace CarSalesSystem.Services
{
	public class DealerService : IDealerService
	{
		private readonly ApplicationDbContext _context;
		public DealerService(ApplicationDbContext context)
		{
			_context = context;
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
	}
}
