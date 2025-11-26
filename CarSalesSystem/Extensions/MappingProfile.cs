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
		}
	}
}
