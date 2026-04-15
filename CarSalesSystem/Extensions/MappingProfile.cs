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
			CreateMap<Car, CarDto>()
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
				.ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer.Name));
			CreateMap<CarDto, Car>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
        }
	}
}
