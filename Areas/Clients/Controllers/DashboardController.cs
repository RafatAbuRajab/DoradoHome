using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "Client")]
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public DashboardController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> ClientDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(client == null)
            {
                return NotFound();
            }

            var ClientData = await _db.Properties
                .Where(x => x.ClientId == client.Id)
                .Include(x => x.Rentals)
                .Include(x => x.Sales)
                .Include(x => x.Appointments)
                .Include(x => x.SavedProperties)
                .ToListAsync();

            ViewBag.SavedProperties = await _db.Properties
                .Where(x => x.SavedProperties.Any(s => s.ClientId == client.Id))
                .ToListAsync();

            ViewBag.PendingAppointmens = _db.Appointments
                .Where(x => x.Status == AppointmentStatus.Pending && x.ClientId == client.Id)
                .Count();

            return View(ClientData);
        }
    }
}
