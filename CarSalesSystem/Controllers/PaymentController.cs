using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Controllers
{
	public class PaymentController : Controller
	{
		private readonly ApplicationDbContext _context;

		public PaymentController(ApplicationDbContext context)
		{
			_context = context; 
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
	}
}
