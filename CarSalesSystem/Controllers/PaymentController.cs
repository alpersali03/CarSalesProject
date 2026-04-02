using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Controllers
{
	public class PaymentController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ICarService _carService;
		private readonly IPaymentService _paymentService;

		public PaymentController(ApplicationDbContext context, ICarService carService, IPaymentService paymentService)
		{
			_context = context;
			_carService = carService;
			_paymentService = paymentService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View(new PaymentDto());
		}

		[HttpPost]
		public IActionResult Add(PaymentDto dto)
		{
			if (!ModelState.IsValid)
				return View(dto);
			try
			{
				_paymentService.Add(dto);
				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpGet]
		public IActionResult Details(int id)
		{
			try
			{
				var payment = _context.Payments
					.Include(p => p.DebitCard)
					.Include(p => p.Car)
					.FirstOrDefault(p => p.Id == id);
				if (payment == null)
					return NotFound();
				return View(payment);
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			try
			{
				var payments = _context.Payments
					.Include(p => p.DebitCard)
					.Include(p => p.Car)
					.ToList();
				return View(payments);
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		// ── BUY ──────────────────────────────────────────────────────────────

		[HttpGet]
		public IActionResult Buy(int carId)
		{
			try
			{
				var car = _carService.GetById(carId);
				if (car == null)
					return NotFound();

				ViewBag.CarId = carId;
				ViewBag.Car = car;

				return View(new PaymentDto { CarId = carId });   // ← fixed
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpPost]
		public IActionResult Buy(PaymentDto dto)
		{
			if(dto.IsBought == true)
			{
				throw new ArgumentException("This car is already bought!");
			}

			if (!ModelState.IsValid)
			{
				var car = _carService.GetById(dto.CarId);
				ViewBag.CarId = dto.CarId;
				ViewBag.Car = car;
				return View(dto);
			}

			try
			{
				// 1. Save the debit card
				string user = User.GetId();
				_paymentService.Buy(dto, user);

				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}
	}
}
