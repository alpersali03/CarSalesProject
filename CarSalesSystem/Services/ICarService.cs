using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface ICarService
	{
		void Add(CarFormDto carDto);
		void Edit(int id, CarFormDto dto);
		Car GetById(int id);

		List<CarDto> Search(string keyword);
		List<CarDto> SortByName(string sortOrder);
		List<CarDto> SortByPrice(string letter);
		List<CarDto> GetLatest(int count);
		List<CarDto> GetByFuel(string fuelType);
		List<CarDto> GetByYear(int? minYear, int? maxYear);


	}
}
