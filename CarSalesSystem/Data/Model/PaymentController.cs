using CarSalesSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSalesSystem.Data.Model
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            context = _context;
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
        [HttpGet]
        public IActionResult Details(int id)
        {
            var payment = _context.Payments
                .Include(d => d.DebitCardId)
                .Include(c => c.Car)
                .FirstOrDefault(c => c.Id == id);

            if (payment == null)
            {
                return NotFound();

            }

            return View(payment);
        }
    }
}
