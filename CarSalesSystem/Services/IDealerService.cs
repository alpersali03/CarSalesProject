using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IDealerService
	{
		bool CheckIsDealerByUserId(string userId);
		List<DealerDto> GetAll();

		void Add(DealerDto dealer);

		void Edit(DealerDto dealer);

		Dealer Details(int id);

		Dealer GetById(int id);

		void Update(Dealer dealer);
	}
}
