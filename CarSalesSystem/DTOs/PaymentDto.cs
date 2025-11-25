namespace CarSalesSystem.DTOs
{
    public class PaymentDto
    {
        public DateTime PaymentTime { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
