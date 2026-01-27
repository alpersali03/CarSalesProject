using System.Security.Claims;


namespace CarSalesSystem.Extensions
{
	public static class ClaimPrincipal
	{

		public static string? GetId(this ClaimsPrincipal user)
		{
			return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		public static bool IsAdmin(this ClaimsPrincipal user)
           => user.IsInRole("Manager");
	}
}
