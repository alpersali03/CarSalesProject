using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
	public class CarIndexConditionDto
	{
		public List<CarDto> Cars { get; set; } = new List<CarDto>();
		public string CarType { get; set; } = null!;
		[Range(1950, 2026, ErrorMessage = "Year must be between 1886 and the current year.")]
		public int Year { get; set; }
	}
}
