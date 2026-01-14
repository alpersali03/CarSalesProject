using CarSalesSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
	[Authorize(Roles = "Manager")]
	public class AdminController : Controller
	{

		private readonly IDealerService _dealerService;
		public AdminController(IDealerService dealerService)
		{
			_dealerService = dealerService;
		}
		[HttpGet]
		public IActionResult GetAllDealers()
		{
			try
			{
				List<DTOs.DealerDto> dealers = _dealerService.GetAll();
				return View(dealers);
			}
			catch (Exception)
			{
				return BadRequest("An error occurred while loading dealers.");
			}

		}
	}
}
