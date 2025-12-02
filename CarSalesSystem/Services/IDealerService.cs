using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IDealerService
	{
		bool CheckIsDealerByUserId(string userId);
		List<DealerDto> GetAll();
	}
}
