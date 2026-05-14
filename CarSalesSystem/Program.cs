using CarSalesSystem.Common;
using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CarSalesSystem
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var configuredConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			var connectionString = NormalizeSupabaseConnectionString(configuredConnectionString);

			builder.Services.Configure<SeedDataOptions>(
				builder.Configuration.GetSection(SeedDataOptions.SectionName));

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseNpgsql(connectionString));

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
			builder.Services.AddScoped<SqlServerToSupabaseImporter>();

			var app = builder.Build();

			if (args.Contains("--import-sqlserver", StringComparer.OrdinalIgnoreCase))
			{
				await ImportFromSqlServerAsync(app, args);
				return;
			}

			await ApplyMigrationsAsync(app);
			await SeedRolesAsync(app);
			await SeedDemoMarketplaceAsync(app);

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

		private static string NormalizeSupabaseConnectionString(string configuredConnectionString)
		{
			if (configuredConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
				|| configuredConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
			{
				var uri = new Uri(configuredConnectionString);
				var userInfo = uri.UserInfo.Split(':', 2);

				if (userInfo.Length != 2 || string.IsNullOrWhiteSpace(userInfo[0]) || string.IsNullOrWhiteSpace(userInfo[1]))
				{
					throw new InvalidOperationException(
						"The Supabase connection string must include both username and password.");
				}

				var database = uri.AbsolutePath.Trim('/');
				if (string.IsNullOrWhiteSpace(database))
				{
					database = "postgres";
				}

				var builder = new NpgsqlConnectionStringBuilder
				{
					Host = uri.Host,
					Port = uri.Port > 0 ? uri.Port : 5432,
					Database = database,
					Username = Uri.UnescapeDataString(userInfo[0]),
					Password = Uri.UnescapeDataString(userInfo[1]),
					SslMode = SslMode.Require
				};

				return builder.ConnectionString;
			}

			var looksLikePostgres = configuredConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
				&& configuredConnectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase);

			if (!looksLikePostgres)
			{
				throw new InvalidOperationException(
					"This application is configured for Supabase/PostgreSQL only. Set ConnectionStrings__DefaultConnection to a Supabase Postgres connection string.");
			}

			return configuredConnectionString;
		}

		private static async Task ApplyMigrationsAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			await dbContext.Database.EnsureCreatedAsync();
		}

		private static async Task ImportFromSqlServerAsync(WebApplication app, string[] args)
		{
			using var scope = app.Services.CreateScope();
			var importer = scope.ServiceProvider.GetRequiredService<SqlServerToSupabaseImporter>();
			var sourceConnection = args
				.FirstOrDefault(arg => arg.StartsWith("--source-connection=", StringComparison.OrdinalIgnoreCase))
				?.Split('=', 2)[1];

			await importer.ImportAsync(sourceConnection);
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

		private static async Task SeedDemoMarketplaceAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			if (await dbContext.Cars.AnyAsync())
			{
				return;
			}

			var categories = await dbContext.Categories
				.OrderBy(c => c.Id)
				.ToListAsync();

			if (categories.Count == 0)
			{
				categories =
				[
					new Category { Name = "Sedan" },
					new Category { Name = "SUV" },
					new Category { Name = "Pickup truck" },
					new Category { Name = "Hatchback" },
					new Category { Name = "Minivan" },
					new Category { Name = "Sports car" }
				];

				dbContext.Categories.AddRange(categories);
				await dbContext.SaveChangesAsync();
			}

			var dealers = new[]
			{
				new Dealer
				{
					Name = "Mira Ivanova",
					CompanyName = "Sofia Auto House",
					PhoneNumber = "+359888111222",
					UserId = "demo-dealer-sofia"
				},
				new Dealer
				{
					Name = "Daniel Petrov",
					CompanyName = "Plovdiv Premium Cars",
					PhoneNumber = "+359888333444",
					UserId = "demo-dealer-plovdiv"
				},
				new Dealer
				{
					Name = "Elena Dimitrova",
					CompanyName = "Varna Select Motors",
					PhoneNumber = "+359888555666",
					UserId = "demo-dealer-varna"
				}
			};

			dbContext.Dealers.AddRange(dealers);
			await dbContext.SaveChangesAsync();

			var sedanId = GetCategoryId(categories, "Sedan");
			var suvId = GetCategoryId(categories, "SUV");
			var hatchbackId = GetCategoryId(categories, "Hatchback");
			var sportsId = GetCategoryId(categories, "Sports car");
			var minivanId = GetCategoryId(categories, "Minivan");

			var cars = new[]
			{
				new Car
				{
					Brand = "BMW",
					Model = "320d xDrive",
					Description = "Clean executive sedan with service history, leather interior, parking sensors, and strong fuel economy.",
					ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=1200&q=80",
					Year = 2021,
					Mileage = 62000,
					FuelType = "Diesel",
					Transmission = "Automatic",
					Price = 48900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Sofia",
					CategoryId = sedanId,
					DealerId = dealers[0].Id
				},
				new Car
				{
					Brand = "Audi",
					Model = "Q5 45 TFSI",
					Description = "Premium SUV with quattro, panoramic roof, digital cockpit, and adaptive cruise control.",
					ImageUrl = "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?auto=format&fit=crop&w=1200&q=80",
					Year = 2022,
					Mileage = 41000,
					FuelType = "Petrol",
					Transmission = "Automatic",
					Price = 68900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Plovdiv",
					CategoryId = suvId,
					DealerId = dealers[1].Id
				},
				new Car
				{
					Brand = "Mercedes-Benz",
					Model = "C 220d AMG Line",
					Description = "Elegant sedan with AMG package, ambient lighting, heated seats, and full maintenance records.",
					ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&w=1200&q=80",
					Year = 2020,
					Mileage = 78000,
					FuelType = "Diesel",
					Transmission = "Automatic",
					Price = 53900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Varna",
					CategoryId = sedanId,
					DealerId = dealers[2].Id
				},
				new Car
				{
					Brand = "Volkswagen",
					Model = "Golf 8 eTSI",
					Description = "Modern hatchback with mild-hybrid engine, lane assist, wireless smartphone integration, and low mileage.",
					ImageUrl = "https://images.unsplash.com/photo-1617814076367-b759c7d7e738?auto=format&fit=crop&w=1200&q=80",
					Year = 2021,
					Mileage = 36000,
					FuelType = "Petrol",
					Transmission = "Automatic",
					Price = 34900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Sofia",
					CategoryId = hatchbackId,
					DealerId = dealers[0].Id
				},
				new Car
				{
					Brand = "Porsche",
					Model = "718 Cayman",
					Description = "Driver-focused sports car with sport chrono, PDK transmission, and excellent condition.",
					ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=1200&q=80",
					Year = 2019,
					Mileage = 29000,
					FuelType = "Petrol",
					Transmission = "Automatic",
					Price = 112000m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Plovdiv",
					CategoryId = sportsId,
					DealerId = dealers[1].Id
				},
				new Car
				{
					Brand = "Toyota",
					Model = "RAV4 Hybrid",
					Description = "Reliable hybrid SUV with excellent economy, adaptive safety systems, and spacious family interior.",
					ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=1200&q=80",
					Year = 2022,
					Mileage = 52000,
					FuelType = "Hybrid",
					Transmission = "Automatic",
					Price = 58900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Varna",
					CategoryId = suvId,
					DealerId = dealers[2].Id
				},
				new Car
				{
					Brand = "Ford",
					Model = "S-Max Titanium",
					Description = "Practical seven-seat minivan with large cargo space, navigation, and comfortable long-distance ride.",
					ImageUrl = "https://images.unsplash.com/photo-1542362567-b07e54358753?auto=format&fit=crop&w=1200&q=80",
					Year = 2018,
					Mileage = 94000,
					FuelType = "Diesel",
					Transmission = "Manual",
					Price = 25900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Sofia",
					CategoryId = minivanId,
					DealerId = dealers[0].Id
				},
				new Car
				{
					Brand = "Tesla",
					Model = "Model 3 Long Range",
					Description = "Electric sedan with dual motor AWD, glass roof, premium audio, and fast charging capability.",
					ImageUrl = "https://images.unsplash.com/photo-1560958089-b8a1929cea89?auto=format&fit=crop&w=1200&q=80",
					Year = 2021,
					Mileage = 48000,
					FuelType = "Electric",
					Transmission = "Automatic",
					Price = 62900m,
					IsListed = true,
					Country = "Bulgaria",
					City = "Varna",
					CategoryId = sedanId,
					DealerId = dealers[2].Id
				}
			};

			dbContext.Cars.AddRange(cars);
			await dbContext.SaveChangesAsync();
		}

		private static int GetCategoryId(List<Category> categories, string name)
		{
			return categories.FirstOrDefault(c => c.Name == name)?.Id
				?? categories[0].Id;
		}
	}
}
