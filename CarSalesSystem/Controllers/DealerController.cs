using CarSalesSystem.Data;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class DealerController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IDealerService _dealerService;

	public DealerController(ApplicationDbContext context, IDealerService dealerService)
	{
		_context = context;
		_dealerService = dealerService;
	}

	public IActionResult Index()
	{
		return View();
	}

	[HttpGet]
	public IActionResult GetAll()
	{
		try
		{
			var dealers = _dealerService.GetAll();
			return View(dealers);
		}
		catch (Exception ex)
		{
			// Log the exception here
			ModelState.AddModelError("", "An error occurred while fetching dealers.");
			return View(new List<DealerDto>());
		}
	}

	[HttpGet]
	public IActionResult Add()
	{
		return View(new DealerDto());
	}

	[HttpPost]
	public IActionResult Add(DealerDto dto)
	{
		try
		{
			var getUserId = User.GetId();
			if (getUserId == null)
			{
				return NotFound();
			}

			_dealerService.Add(dto);
			return RedirectToAction("GetAll");
		}
		catch (Exception ex)
		{
			// Log exception
			ModelState.AddModelError("", "An error occurred while adding the dealer.");
			return View(dto);
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
				return NotFound();
			}

			var dto = new DealerDto
			{
				Id = dealer.Id,
				Name = dealer.Name,
				CompanyName = dealer.CompanyName,
				PhoneNumber = dealer.PhoneNumber,
				UserId = dealer.UserId
			};

			return View(dto);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "An error occurred while loading the dealer.");
			return RedirectToAction("GetAll");
		}
	}

	[HttpPost]
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

			// Update fields
			dealer.Name = dealerDto.Name;
			dealer.CompanyName = dealerDto.CompanyName;
			dealer.PhoneNumber = dealerDto.PhoneNumber;

			_dealerService.Update(dealer);

			return RedirectToAction("GetAll");
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "An error occurred while editing the dealer.");
			return View(dealerDto);
		}
	}

	[HttpGet]
	public IActionResult Details(int id)
	{
		try
		{
			var dealer = _context.Dealers.Include(d => d.Cars).FirstOrDefault(d => d.Id == id);
			if (dealer == null)
			{
				return NotFound();
			}
			return View(dealer);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "An error occurred while fetching dealer details.");
			return RedirectToAction("GetAll");
		}
	}
}
