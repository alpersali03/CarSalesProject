using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using CarSalesSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Controllers
{
    public class DealerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DealerService _dealerService;
        public DealerController(ApplicationDbContext context, DealerService dealerService)
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
            var dealers = _dealerService.GetAll();
            return View(dealers); 

        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new DealerDto());
        }
        [HttpPost]
        public IActionResult Add(DealerDto dto)
        {
            _dealerService.Add(dto);
            return RedirectToAction("GetAll");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var dealer = _context.Dealers.FirstOrDefault(d => d.Id == id);
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
                UserId = dealer.UserId,
            };
            return View(dto);
        }
        [HttpPost]
        public IActionResult Edit(int id, DealerDto dealerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(dealerDto);
            }
			var getUserId = User.GetId();
			if (getUserId == null)
			{
				return NotFound();
			}
			var dealer = _context.Dealers.FirstOrDefault(d => d.Id == id);
            if (dealer == null)
            {
                return NotFound();
            }
            dealer.Name = dealerDto.Name;
            dealer.CompanyName = dealerDto.CompanyName;
            dealer.PhoneNumber = dealerDto.PhoneNumber;

            _context.SaveChanges();
            return RedirectToAction("GetAll");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var dealer = _context.Dealers.Include(d=>d.Cars).FirstOrDefault(d=> d.Id == id);
            if(dealer == null)
            {
                return NotFound();
            }
            return View(dealer);
        }
        
    }
}
