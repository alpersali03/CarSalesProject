using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface IFavoriteService
	{
		void ToggleFavorite(string userId, int carId);

		HashSet<int> GetFavoriteCarIds(string? userId);

		List<CarDto> GetFavoriteCars(string userId);

		int CountFavoritesForDealer(int dealerId);
	}
}
