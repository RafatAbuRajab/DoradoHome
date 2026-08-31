using DoradoHome.Areas.Agent.Models;
using DoradoHome.Data;
using DoradoHome.Models;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
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

        public async Task<IActionResult> AgentDashboard()
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

            var AgentData = await _db.Properties
                .Where(x => x.EmployeeId == agent.Id)
                .Include(x => x.PropertyImages)
                .Include(x => x.Appointments)
                .ThenInclude(x => x.Client).Include(x => x.Sales)
                .Include(x => x.Rentals)
                .ToListAsync();

            return View(AgentData);
        }
    }
}
