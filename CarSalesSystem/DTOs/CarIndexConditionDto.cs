using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
	public class CarIndexConditionDto
	{
		public List<CarDto> Cars { get; set; } = new List<CarDto>();
		public string CarType { get; set; } = null!;
		
		public int Year { get; set; }
	}

}
