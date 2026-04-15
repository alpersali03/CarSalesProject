using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Models;

namespace CarSalesSystem.Services
{
	public interface ICarService
	{
		void Add(CarFormDto carDto);

		void Edit(int id, CarFormDto dto, string userId);

		Car? GetById(int id);

		List<Car> GetByIds(IEnumerable<int> ids);

		List<CarDto> Search(string keyword);

		List<CarDto> SortByName(string sortOrder);

		List<CarDto> SortByPrice(string letter);

		List<CarDto> GetLatest(int count);

		List<CarDto> GetByFuel(string fuelType);

		List<CarDto> GetByYear(int? minYear, int? maxYear);

		List<CarDto> GetByBrand(string brandType);

		List<CarDto> Search(CarSearchViewModel search);

		void Delete(string userId, int id);

		void Details(int id);

		List<string> PopulateBrands();

		List<CarDto> GetAll();
	}
}
