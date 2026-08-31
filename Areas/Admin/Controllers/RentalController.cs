using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RentalController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RentalController(AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Rental()
        {
            var rental = await _db.Rentals
                        .Include(p => p.Property)
                        .Include(c => c.Client)
                        .Include(e => e.Employee).ToListAsync();

            bool changed = false;
            foreach (var item in rental)
            {
                var RentRemain = (item.LeaseEndDate - DateTime.Today).Days;
                var RentEnded = item.LeaseStartDate == item.LeaseEndDate;

                if (RentEnded)
                {
                    item.LeaseStatus = LeaseStatus.Ended;
                    changed = true;
                }
                if(RentRemain > 30 && item.LeaseStatus != LeaseStatus.Active)
                {
                    item.LeaseStatus = LeaseStatus.Active;
                    changed = true;
                }
                else if(RentRemain < 30 && RentRemain > 0 && item.LeaseStatus != LeaseStatus.EndingSoon)
                {
                    item.LeaseStatus = LeaseStatus.EndingSoon;
                    changed = true;
                }
                else if(RentRemain <= 0 && item.LeaseStatus != LeaseStatus.Expired && RentEnded == false)
                {
                    item.LeaseStatus = LeaseStatus.Expired;
                    item.Property.IsRented = false;
                    changed = true;
                }
            }
            if(changed)
            {
                await _db.SaveChangesAsync();
            }
            return View(rental);
        }

        [HttpGet]
        public IActionResult AddRent()
        {
            ViewBag.prop = new SelectList(_db.Properties.Where(x => x.OfferType == OfferType.ForRent && x.IsRented == false), "Id", "Name");
            ViewBag.client = new SelectList(_db.Clients, "Id", "FullName");
            ViewBag.Emp = new SelectList(_db.Employees, "Id", "FullName");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRent(Rental model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                ViewBag.prop = new SelectList(_db.Properties.Where(x => x.OfferType == OfferType.ForRent && x.IsRented == false), "Id", "Name");
                ViewBag.client = new SelectList(_db.Clients, "Id", "FullName");
                ViewBag.Emp = new SelectList(_db.Employees, "Id", "FullName");
                return View(model);
            }

            var rent = new Rental
            {
                LeaseStartDate = model.LeaseStartDate,
                LeaseEndDate = model.LeaseEndDate,
                MonthlyRentPrice = model.MonthlyRentPrice,
                SecurityDepositPrice = model.SecurityDepositPrice,
                Notes = model.Notes,
                LeaseStatus = model.LeaseStatus,
                PropertyId = model.PropertyId,
                ClientId = model.ClientId,
                EmployeeId = model.EmployeeId,
                IsRented = true,
                IsActive = true
            };

            var property = await _db.Properties.FindAsync(model.PropertyId);
            if(property == null)
            {
                return NotFound();
            }

            property.ClientId = model.ClientId;
            property.IsRented = true;
            property.Status = PropertyStatus.Sold;

            await _db.Rentals.AddAsync(rent);
            _db.Properties.Update(property);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Rental));
        }

        [HttpGet]
        public async Task<IActionResult> EditRent(int id)
        {
            var rent = await _db.Rentals.FindAsync(id);
            ViewBag.prop = new SelectList(_db.Properties, "Id", "Name");
            ViewBag.client = new SelectList(_db.Clients, "Id", "FullName");
            ViewBag.Emp = new SelectList(_db.Employees, "Id", "FullName");

            return View(rent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRent(Rental model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var rent = await _db.Rentals.FindAsync(model.Id);
            var prop = await _db.Properties.FindAsync(rent.PropertyId);

            if(rent == null || prop == null)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            rent.LeaseStartDate = model.LeaseStartDate;
            rent.LeaseEndDate = model.LeaseEndDate;
            rent.MonthlyRentPrice = model.MonthlyRentPrice;
            rent.SecurityDepositPrice = model.SecurityDepositPrice;
            rent.LeaseStatus = LeaseStatus.PendingSignature;
            rent.PropertyId = model.PropertyId;
            rent.ClientId = model.ClientId;
            rent.EmployeeId = model.EmployeeId;
            prop.ClientId = model.ClientId;
            prop.IsRented = true;
            prop.Status = PropertyStatus.Sold;
            rent.IsRented = true;
            rent.IsActive = true;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Rental));
        }

        public async Task<IActionResult> EndRent(int id)
        {
            var rent = await _db.Rentals.FindAsync(id);
            rent.LeaseStatus = LeaseStatus.Ended;
            rent.LeaseEndDate = rent.LeaseStartDate;
            rent.IsRented = false;
            rent.IsActive = false;
            var property = await _db.Properties.FindAsync(rent.PropertyId);
            property.IsRented = false;
            property.Status = PropertyStatus.Active;

            _db.Properties.Update(property);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Rental));
        }
    }
}
