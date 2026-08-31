using DoradoHome.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "Client")]
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
            if (user == null)
            {
                return NotFound();
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (client == null)
            {
                return NotFound();
            }

            var ClientData = await _db.Properties
                .Where(x => x.ClientId == client.Id)
                .Include(x => x.Rentals)
                .ToListAsync();

            return View(ClientData);
        }
    }
}
