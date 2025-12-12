using CarSalesSystem.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Car> Cars { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Dealer> Dealers { get; set; }
		public DbSet<DebitCard> DebitCards { get; set; }
		public DbSet<Payment> Payments { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Category>().HasData
				(
				new Category { Id = 1, Name = "Sedan" },
				new Category { Id = 2, Name = "SUV" },
				new Category { Id = 3, Name = "Pickup truck" },
				new Category { Id = 4, Name = "Hatchback" },
				new Category { Id = 5, Name = "Minivan" },
				new Category { Id = 6, Name = "Sports car" }

				);
		}
	}
}
