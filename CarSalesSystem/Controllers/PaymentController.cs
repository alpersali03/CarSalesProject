using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Controllers
{
	public class PaymentController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ICarService _carService;

		public PaymentController(ApplicationDbContext context, ICarService carService)
		{
			_context = context;
			_carService = carService;
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
				var payment = new Payment
				{
					PaymentTime = dto.PaymentTime,
					TotalAmount = dto.TotalAmount,
					IsSuccessful = dto.IsSuccessful,
				};
				_context.Payments.Add(payment);
				_context.SaveChanges();
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
				var car = _context.Cars.FirstOrDefault(c => c.Id == carId);
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
			if (!ModelState.IsValid)
			{
				var car = _context.Cars.FirstOrDefault(c => c.Id == dto.CarId);
				ViewBag.CarId = dto.CarId;
				ViewBag.Car = car;
				return View(dto);
			}

			try
			{
				// 1. Save the debit card
				var debitCard = new DebitCard
				{
					CardNumber = dto.CardNumber,
					CVV = dto.CVV,
					FullName = dto.FullName,
					ExpirationMonth = dto.ExpirationMonth,
					ExpirationYear = dto.ExpirationYear,
				};
				_context.DebitCards.Add(debitCard);
				_context.SaveChanges();

				// 2. Look up car price
				var car = _context.Cars.FirstOrDefault(c => c.Id == dto.CarId);

				// 3. Save the payment
				var payment = new Payment
				{
					PaymentTime = DateTime.Now,
					TotalAmount = car?.Price ?? 0,
					IsSuccessful = true,
					DebitCardId = debitCard.Id,
					CarId = dto.CarId,
				};
				_context.Payments.Add(payment);
				_context.SaveChanges();

				return RedirectToAction("Details", new { id = payment.Id });
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}
	}
}
