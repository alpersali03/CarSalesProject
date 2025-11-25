using System.ComponentModel.DataAnnotations;

namespace CarSalesSystem.Data.Model
{
    public class DebitCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CardNumber { get; set; } = null!;

        [Required]
        public int CVV { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string ExpirationMonth { get; set; } = null!;

        [Required]
        public int ExpirationYear { get; set; }

        public List<Payment> Payments { get; set; } = new();
    }

}
