using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.DTOs
{
    public class CarFormDto
    {
        [Required]
        public string Brand { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public int Year { get; set; }

        public int Mileage { get; set; }

        [Required]
        public string FuelType { get; set; } = null!;

        [Required]
        public string Transmission { get; set; } = null!;

        public decimal Price { get; set; }

        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;

        public int CategoryId { get; set; }
        public int DealerId { get; set; }

        public bool IsListed { get; set; }
    }

}
