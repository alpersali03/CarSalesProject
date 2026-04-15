using CarSalesSystem.Data;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
	public class DealerController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IDealerService _dealerService;

		public DealerController(ApplicationDbContext context, IDealerService dealerService)
		{
			_context = context;
			_dealerService = dealerService;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			try
			{
				var dealers = _dealerService.GetAll();
				return View(dealers);
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred while fetching dealers.");
				return View(new List<DealerDto>());
			}
		}

		[HttpGet]
		[Authorize]
		public IActionResult Add()
		{
			var userId = User.GetId();
			if (_dealerService.CheckIsDealerByUserId(userId!))
			{
				TempData["Error"] = "You are already registered as a dealer.";
				return RedirectToAction(nameof(MyDashboard));
			}

			return View(new DealerDto());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Add(DealerDto dto)
		{
			try
			{
				var userId = User.GetId();
				if (userId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				if (_dealerService.CheckIsDealerByUserId(userId))
				{
					TempData["Error"] = "You are already registered as a dealer.";
					return RedirectToAction(nameof(MyDashboard));
				}

				dto.UserId = userId;
				_dealerService.Add(dto);
				return RedirectToAction(nameof(MyDashboard));
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred while adding the dealer.");
				return View(dto);
			}
		}

		[HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
		{
			try
			{
				var dealer = _dealerService.GetById(id);
				if (dealer == null)
				{
					return NotFound();
				}

				if (dealer.UserId != User.GetId())
				{
					return Forbid();
				}

				return View(new DealerDto
				{
					Id = dealer.Id,
					Name = dealer.Name,
					CompanyName = dealer.CompanyName,
					PhoneNumber = dealer.PhoneNumber,
					UserId = dealer.UserId
				});
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred while loading the dealer.");
				return RedirectToAction(nameof(GetAll));
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int id, DealerDto dealerDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(dealerDto);
				}

				var currentUserId = User.GetId();
				if (currentUserId == null)
				{
					return Unauthorized();
				}

				var dealer = _dealerService.GetById(id);
				if (dealer == null)
				{
					return NotFound();
				}

				if (dealer.UserId != currentUserId)
				{
					return Forbid();
				}

				dealer.Name = dealerDto.Name;
				dealer.CompanyName = dealerDto.CompanyName;
				dealer.PhoneNumber = dealerDto.PhoneNumber;

				_dealerService.Update(dealer);

				return RedirectToAction(nameof(MyDashboard));
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An error occurred while editing the dealer.");
				return View(dealerDto);
			}
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			var profile = _dealerService.GetPublicProfile(id);
			if (profile == null)
			{
				return NotFound();
			}

			return View(profile);
		}

		[HttpGet]
		[Authorize]
		public IActionResult MyDashboard()
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			try
			{
				var dashboard = _dealerService.GetDashboard(userId);
				return View(dashboard);
			}
			catch (UnauthorizedAccessException)
			{
				return RedirectToAction(nameof(Add));
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult ToggleListingStatus(int carId)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			try
			{
				_dealerService.ToggleListingStatus(userId, carId);
				return RedirectToAction(nameof(MyDashboard));
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
			catch (ArgumentException)
			{
				return NotFound();
			}
		}
	}
}
