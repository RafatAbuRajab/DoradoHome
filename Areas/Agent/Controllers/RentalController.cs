using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
    public class RentalController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public RentalController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Rentals()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }

            var agent = await _db.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(agent == null)
            {
                return NotFound();
            }

            var AgentData = await _db.Rentals
                .Where(x => x.EmployeeId == agent.Id && x.IsActive)
                .Include(x => x.Property)
                .Include(x => x.Client)
                .ToListAsync();

            return View(AgentData);
        }

        [HttpGet]
        public async Task<IActionResult> Rent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var agent = await _db.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (agent == null)
            {
                return NotFound();
            }

            ViewBag.Properties = new SelectList(_db.Properties.Where(x => x.IsRented == false && x.OfferType == OfferType.ForRent), "Id", "Name");
            ViewBag.Clients = new SelectList(_db.Clients, "Id", "FullName");

            var rental = new Rental
            {
                Employee = agent,
                EmployeeId = agent.Id
            };

            return View(rental);
        }

        [HttpPost]
        public async Task<IActionResult> Rent(Rental model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                ViewBag.Properties = new SelectList(_db.Properties.Where(x => x.IsRented == false && x.OfferType == OfferType.ForRent), "Id", "Name");
                ViewBag.Clients = new SelectList(_db.Clients, "Id", "FullName");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var agent = await _db.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (agent == null)
            {
                return NotFound();
            }

            var rental = new Rental
            {
                PropertyId = model.PropertyId,
                ClientId = model.ClientId,
                Employee = agent,
                EmployeeId = agent.Id,
                LeaseStartDate = model.LeaseStartDate,
                LeaseEndDate = model.LeaseEndDate,
                MonthlyRentPrice = model.MonthlyRentPrice,
                SecurityDepositPrice = model.SecurityDepositPrice,
                Notes = model.Notes,
                LeaseStatus = model.LeaseStatus,
                IsRented = true
            };

            await _db.Rentals.AddAsync(rental);

            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == rental.PropertyId);
            if(property == null)
            {
                return NotFound();
            }
            property.ClientId = model.ClientId;
            property.IsRented = true;
            property.Status = PropertyStatus.Sold;

            _db.Properties.Update(property);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Rentals));
        }
    }
}
