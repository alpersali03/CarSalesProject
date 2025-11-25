namespace CarSalesSystem.DTOs
{
    public class DebitCardDto
    {
        public string CardNumber { get; set; } = null!;

   
        public int CVV { get; set; }

      
        public string FullName { get; set; } = null!;

        
        public string ExpirationMonth { get; set; } = null!;

        
        public int ExpirationYear { get; set; }
    }
}
