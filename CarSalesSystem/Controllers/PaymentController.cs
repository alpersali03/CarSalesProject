using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
	[Authorize]
	public class PaymentController : Controller
	{
		private readonly ICarService _carService;
		private readonly IPaymentService _paymentService;

		public PaymentController(ICarService carService, IPaymentService paymentService)
		{
			_carService = carService;
			_paymentService = paymentService;
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			var payment = _paymentService.GetPaymentForUser(id, userId, User.IsAdmin());
			if (payment == null)
			{
				return NotFound();
			}

			return View(payment);
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			var payments = _paymentService.GetPaymentsForUser(userId, User.IsAdmin());
			return View(payments);
		}

		[HttpGet]
		public IActionResult Buy(int carId)
		{
			var car = _carService.GetById(carId);
			if (car == null)
			{
				return NotFound();
			}

			if (car.IsBought)
			{
				return BadRequest("This car has already been sold.");
			}

			ViewBag.Car = car;
			return View(new PaymentDto { CarId = carId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Buy(PaymentDto dto)
		{
			var userId = User.GetId();
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Challenge();
			}

			var car = _carService.GetById(dto.CarId);
			if (car == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				ViewBag.Car = car;
				return View(dto);
			}

			var result = _paymentService.Buy(dto, userId);
			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Payment failed.");
				ViewBag.Car = car;
				return View(dto);
			}

			return RedirectToAction(nameof(GetAll));
		}
	}
}
