using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;

namespace CarSalesSystem.Services
{
	public interface ICarService
	{
		void Add(CarFormDto carDto);
		void Edit(int id, CarFormDto dto);
		Car GetById(int id);
	}
}
