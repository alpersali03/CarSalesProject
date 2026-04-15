using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Models;

namespace CarSalesSystem.Services
{
	public interface IDealerService
	{
		void Add(DealerDto dto);

		bool CheckIsDealerByUserId(string userId);

		Dealer? GetById(int id);

		List<DealerDto> GetAll();

		void Update(Dealer dealer);

		Dealer Details(int id);

		void Edit(DealerDto dto);

		Dealer? GetDealerByUserId(string userId);

		void Delete(int id);

		List<Car> GetAllCars(int dealerId);

		DealerDashboardViewModel GetDashboard(string userId);

		DealerPublicProfileViewModel? GetPublicProfile(int dealerId);

		void ToggleListingStatus(string userId, int carId);
	}
}
