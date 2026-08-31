using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AppointmentController(AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Appointment()
        {
            var appointments = await _db.Appointments
                .Include(x => x.Client).Include(p => p.Property)
                .Include(e => e.Employee).ToListAsync();

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Reschedule(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
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

            return RedirectToAction(nameof(Appointment));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Appointment));
        }
    }
}
