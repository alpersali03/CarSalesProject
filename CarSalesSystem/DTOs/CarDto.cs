namespace CarSalesSystem.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string City { get; set; } = null!;
        public int Year { get; set; }

        public int Mileage { get; set; }


        public string FuelType { get; set; } = null!;

		public int DealerId { get; set; }

        public bool IsBought { get; set; }

		public bool IsListed { get; set; }

		public string Country { get; set; } = null!;

		public string CategoryName { get; set; } = null!;

		public string DealerName { get; set; } = null!;  

	}
}
