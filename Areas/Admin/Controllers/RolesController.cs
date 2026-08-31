using DoradoHome.Areas.Admin.Models;
using DoradoHome.Data;
using DoradoHome.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, 
            AppDbContext db,
            RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.TotalRoles = roles.Count;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            ViewBag.Users = new SelectList(_db.Users, "Email", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string Role, string Description, string email)
        {
            if (string.IsNullOrEmpty(Role))
                return View();

            if (await _roleManager.RoleExistsAsync(Role))
                return View();

            await _roleManager.CreateAsync(new AppRole
            {
                Name = Role,
                Description = Description
            });

            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                await _userManager.AddToRoleAsync(user,Role);
            }
            
            return RedirectToAction(nameof(Roles));
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var roles = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(roles);

            return RedirectToAction(nameof(Roles));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string Assignedid)
        {
            var role =await _roleManager.FindByIdAsync(Assignedid);
            ViewBag.Users = new SelectList(_db.Users, "Email", "FullName");
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AppRole model,EmployeeVM employeeVM, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (user == null || role == null)
                return View();
            if (await _userManager.IsInRoleAsync(user, role.ToString()))
                return View();

            var result = await _userManager.AddToRoleAsync(user, role.ToString());

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", "");
                }
                return View();
            }

            if (role.Name == "Admin" || role.Name == "Agent")
            {
                var Employee = new Employee
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    IsActive = true
                };
                await _db.Employees.AddAsync(Employee);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Roles));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(AppRole model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role =await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
                return View(model);

            role.Name = model.Name;
            role.Description = model.Description;

            var result = await _roleManager.UpdateAsync(role);

            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", "");
                }
                return View(model);
            }

            return RedirectToAction(nameof(Roles));
        }
    }
}
