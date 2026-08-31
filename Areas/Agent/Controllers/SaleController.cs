using DoradoHome.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
    public class SaleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public SaleController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> SalesHistory()
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

            var agentData = await _db.Sales
                .Where(x => x.EmployeeId == agent.Id)
                .Include(x => x.Property)
                .Include(x => x.Client)
                .ToListAsync();

            return View(agentData);
        }
    }
}
