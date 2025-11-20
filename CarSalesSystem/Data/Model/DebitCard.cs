using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class DebitCard
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public long CreditCardNumber { get; set; }

        [Required]
        public int CVV { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string ExpMonth { get; set; } = null!;

        [Required]
        public int ExpYear { get; set; }

        public List<Payment>? Payments { get; set; } = new List<Payment>();
    }
}
