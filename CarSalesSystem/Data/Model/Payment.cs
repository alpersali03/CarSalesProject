using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public DateTime PaymentTime { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } 

        [Required]
        public int DebitCardId { get; set; }
        public DebitCard DebitCard { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        public bool IsSuccessful { get; set; } 
    }

}
