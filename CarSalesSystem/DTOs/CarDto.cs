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
    }
}
