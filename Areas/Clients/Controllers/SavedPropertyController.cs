using DoradoHome.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "Client")]
    public class SavedPropertyController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public SavedPropertyController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Saved()
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

            var ClientData = await _db.SavedProperties
                .Where(x => x.ClientId == client.Id)
                .Include(x => x.Property)
                .ToListAsync();

            return View(ClientData);
        }

        public async Task<IActionResult> Remove(int id)
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

            var saved = await _db.SavedProperties
                .FirstOrDefaultAsync(x => x.ClientId == client.Id && x.PropertyId == id);
            if(saved == null)
            {
                return NotFound();
            }

            _db.SavedProperties.Remove(saved);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Saved));
        }
    }
}
