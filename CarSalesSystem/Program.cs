using CarSalesSystem.Common;
using CarSalesSystem.Data;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CarSalesSystem
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			var isSqlite = IsSqliteConnection(connectionString);

			builder.Services.Configure<SeedDataOptions>(
				builder.Configuration.GetSection(SeedDataOptions.SectionName));

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				if (isSqlite)
				{
					options.UseSqlite(connectionString);
				}
				else
				{
					options.UseSqlServer(connectionString);
				}
			});

			builder.Services
				.AddDefaultIdentity<IdentityUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = true;
				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			builder.Services.AddControllersWithViews();
			builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddTransient<IDealerService, DealerService>();
			builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddScoped<ICarService, CarService>();
			builder.Services.AddScoped<IPaymentService, PaymentService>();
			builder.Services.AddScoped<IFavoriteService, FavoriteService>();

			var app = builder.Build();

			await ApplyMigrationsAsync(app);
			await SeedRolesAsync(app);

			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			await app.RunAsync();
		}

		private static bool IsSqliteConnection(string connectionString)
		{
			return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
				|| connectionString.Contains("Filename=", StringComparison.OrdinalIgnoreCase);
		}

		private static async Task ApplyMigrationsAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await dbContext.Database.MigrateAsync();
		}

		private static async Task SeedRolesAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
			var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedDataOptions>>().Value;

			var roles = seedOptions.Roles.Length > 0
				? seedOptions.Roles
				: ["Manager", "Dealer"];

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			if (string.IsNullOrWhiteSpace(seedOptions.AdminEmail))
			{
				return;
			}

			var adminUser = await userManager.FindByEmailAsync(seedOptions.AdminEmail);
			if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Manager"))
			{
				await userManager.AddToRoleAsync(adminUser, "Manager");
			}
		}
	}
}
