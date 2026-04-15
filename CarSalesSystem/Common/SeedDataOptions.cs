namespace CarSalesSystem.Common
{
	public class SeedDataOptions
	{
		public const string SectionName = "SeedData";

		public string[] Roles { get; set; } = [];

		public string? AdminEmail { get; set; }
	}
}
