using CarSalesSystem.Data;
using CarSalesSystem.Data.Model;
using CarSalesSystem.DTOs;
using CarSalesSystem.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Controllers
{
    public class DealerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DealerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var dealer = _context.Dealers.Include(d=>d.Cars).Select(d=> new DealerDto
            {
                Id = d.Id, 
                Name = d.Name,
                CompanyName = d.CompanyName,
                PhoneNumber = d.PhoneNumber,
                UserId = d.UserId,
            }).ToList();
            return View(dealer); 

        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new DealerDto());
        }
        [HttpPost]
        public IActionResult Add(DealerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var getUserId = User.GetId();
            if(getUserId == null)
            {
                return NotFound();
            }
            var delaer = new Dealer
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                PhoneNumber = dto.PhoneNumber,
                UserId = getUserId,
            };
            _context.Dealers.Add(delaer);
            _context.SaveChanges();
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
