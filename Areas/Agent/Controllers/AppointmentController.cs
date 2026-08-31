using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
    public class AppointmentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public AppointmentController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Appointments()
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

            var AppointmentData = await _db.Appointments
                .Where(x => x.EmployeeId == agent.Id)
                .Include(x => x.Property)
                .Include(x => x.Client)
                .ToListAsync();

            return View(AppointmentData);
        }

        [HttpGet]
        public async Task<IActionResult> Offer(int id)
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

            var AppointmentData = await _db.Appointments.FindAsync(id);
            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == AppointmentData.ClientId);
            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == AppointmentData.PropertyId);

            var Offer = new Offer
            {
                Property = property,
                PropertyId = property.Id,
                Client = client,
                ClientId = client.Id,
                Appointment = AppointmentData,
                AppointmentId = AppointmentData.Id
            };

            return View(Offer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Offer(Offer model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
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

            var AppointmentData = await _db.Appointments.FindAsync(model.AppointmentId);
            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == AppointmentData.ClientId);
            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == AppointmentData.PropertyId);
            var employee = await _db.Employees.FirstOrDefaultAsync(x => x.Id == AppointmentData.EmployeeId);

            var offer = new Offer
            {
                OfferType = property.OfferType,
                PropertyId = property.Id,
                EmployeeId = employee.Id,
                ClientId = client.Id,
                AppointmentId = model.AppointmentId,
                SalePrice = model.SalePrice,
                ClosingDate = model.ClosingDate,
                MonthlyRent = model.MonthlyRent,
                LeaseStart = model.LeaseStart,
                LeaseTerm = model.LeaseTerm,
                Notes = model.Notes,
                Status = OfferStatus.Pending,
                CreatedAt = DateTime.Today
            };

            if(offer == null)
            {
                return View(model);
            }

            AppointmentData.Status = AppointmentStatus.PendingConfirmation;

            _db.Appointments.Update(AppointmentData);
            await _db.Offers.AddAsync(offer);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Appointments));
        }

        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if(appointment == null)
            {
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Rejected;

            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Appointments));
        }

        [HttpGet]
        public async Task<IActionResult> Reschedule(int id)
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

            var appointment = await _db.Appointments.FindAsync(id);
            if(appointment == null)
            {
                return RedirectToAction(nameof(Appointments));
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(Appointment model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var appointment = await _db.Appointments.FindAsync(model.Id);

            if (appointment == null)
                return NotFound();

            appointment.Date = model.Date;
            appointment.Time = model.Time;

            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Appointments));
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var appointemnt = await _db.Appointments.FindAsync(id);
            if (appointemnt == null)
                return RedirectToAction(nameof(Appointments));

            _db.Appointments.Remove(appointemnt);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Appointments));
        }
    }
}
