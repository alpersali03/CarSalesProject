using AutoMapper;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;



namespace CarSalesSystem.Extensions

{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Dealer, DealerDto>();
			CreateMap<DealerDto, Dealer>();
			CreateMap<Car, CarFormDto>();
			CreateMap<CarFormDto, Car>();
			CreateMap<Car, CarDto>();
			CreateMap<CarDto, Car>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
        }
	}
}
