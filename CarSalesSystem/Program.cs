using CarSalesSystem.Data;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CarSalesSystem
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services
	.AddDefaultIdentity<IdentityUser>(options =>
	{
		options.SignIn.RequireConfirmedAccount = true;
	})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

			builder.Services.AddControllersWithViews();
			builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddTransient<IDealerService, DealerService>();
			builder.Services.AddScoped<ICarService, CarService>();

			var app = builder.Build();

			// 🔽 ROLE SEEDING HERE
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

				string[] roles = { "Manager", "Dealer", "Customer" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}

				var adminEmail = "admin@carsales.com";
				var adminUser = await userManager.FindByEmailAsync(adminEmail);

				if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Manager"))
				{
					await userManager.AddToRoleAsync(adminUser, "Manager");
				}
			}


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			await app.RunAsync();
		}
	}
}
