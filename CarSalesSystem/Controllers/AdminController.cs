using AutoMapper;
using CarSalesSystem.Data.Model;
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
		private readonly IMapper _mapper;
		public AdminController(IDealerService dealerService, IMapper mapper)
		{
			_dealerService = dealerService;
			_mapper = mapper;
		}
		[HttpGet]
		public IActionResult GetAllDealers()
		{
			try
			{
				List<DTOs.DealerDto> dealers = _dealerService.GetAll();
				return View(dealers);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}


		}
		[HttpGet]
		public IActionResult Edit(int id)
		{
			try
			{
				var dealer = _dealerService.GetById(id);

				if (dealer == null)
				{
					return NotFound("Dealer not found.");
				}

				var dealerDto = _mapper.Map<DealerDto>(dealer);
				return View(dealerDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}


		[HttpPost]
		public IActionResult Edit(DealerDto dealer)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(dealer);
				}

				_dealerService.Edit(dealer);

				return RedirectToAction(nameof(GetAllDealers));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpGet]
		public IActionResult Delete(int id)
		{
			try
			{
				var dealer = _dealerService.GetById(id);

				if (dealer == null)
				{
					return NotFound("Dealer not found.");
				}

				var dealerDto = _mapper.Map<DealerDto>(dealer);
				return View(dealerDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IActionResult DeleteConfirmed(int id)
		{
			try
			{
				_dealerService.Delete(id);
				return RedirectToAction(nameof(GetAllDealers));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet]	
		public IActionResult GetAllCars(int dealerId)
		{
			try
			{
				var cars = _dealerService.GetAllCars(dealerId);
				return View(cars);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
