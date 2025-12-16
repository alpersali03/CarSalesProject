using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesSystem.Controllers
{
	public class DebitCardController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DebitCardController(ApplicationDbContext context)
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
			return View(new DebitCardDto());
		}

		[HttpPost]
		public IActionResult Add(DebitCardDto dto)
		{
			if (!ModelState.IsValid)
				return View(dto);

			try
			{
				var debit = new DebitCard
				{
					CardNumber = dto.CardNumber,
					CVV = dto.CVV,
					FullName = dto.FullName,
					ExpirationMonth = dto.ExpirationMonth,
					ExpirationYear = dto.ExpirationYear,
				};

				_context.DebitCards.Add(debit);
				_context.SaveChanges();

				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			try
			{
				var debitCard = _context.DebitCards.Find(id);
				if (debitCard == null)
				{
					return NotFound();
				}

				var dto = new DebitCardDto
				{
					CardNumber = debitCard.CardNumber,
					CVV = debitCard.CVV,
					FullName = debitCard.FullName,
					ExpirationMonth = debitCard.ExpirationMonth,
					ExpirationYear = debitCard.ExpirationYear,
				};

				return View(dto);
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpPost]
		public IActionResult Edit(int id, DebitCardDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return View(dto);

				var debit = _context.DebitCards.Find(id);
				if (debit == null)
					return NotFound();

				debit.CardNumber = dto.CardNumber;
				debit.CVV = dto.CVV;
				debit.FullName = dto.FullName;
				debit.ExpirationMonth = dto.ExpirationMonth;
				debit.ExpirationYear = dto.ExpirationYear;

				_context.SaveChanges();

				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			try
			{
				var debit = _context.DebitCards.Find(id);
				if (debit == null)
					return NotFound();

				_context.DebitCards.Remove(debit);
				_context.SaveChanges();

				return RedirectToAction("GetAll");
			}
			catch (Exception)
			{
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var cards = _context.DebitCards.ToList();
			return View(cards);
		}
	}
}
