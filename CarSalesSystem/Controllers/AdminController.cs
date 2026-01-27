using CarSalesSystem.DTOs;
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
		[HttpGet]
		public IActionResult Edit(int id)
		{
			try
			{
				var dealer = _dealerService.GetById(id);
				return View(dealer);
			}
			catch (Exception)
			{
				return BadRequest("Dealer not found!");
			}
		}
		[HttpPost]
		public IActionResult Edit(DealerDto dealerDto)
		{
			//try
			//{
			//	_dealerService.Update(dealerDto);
			//	return RedirectToAction("Edit");
			//}
			//catch (Exception)
			//{
			//	return BadRequest("An error occurred while editing the dealer.");
			//}
			return View();
		}
		//public IActionResult Delete(int id)
		//{
		//	try
		//	{
		//		DealerDto dealer = _dealerService.Delete(id);
		//		return View(dealer);
		//	}
		//	catch (Exception)
		//	{
		//		return BadRequest("An error occurred while loading the dealer for deletion.");
		//	}
		//}
	}
}
