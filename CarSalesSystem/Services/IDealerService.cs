using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IDealerService
	{
		bool CheckIsDealerByUserId(string userId);
		List<DealerDto> GetAll();

		void Add(DealerDto dealer);

		void Edit(DealerDto dealer);

		void Details(int id);
	}
}
