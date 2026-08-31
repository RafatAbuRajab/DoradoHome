using DoradoHome.Areas.Admin.Models;
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
    public class AgentController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public AgentController(AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        public async Task<IActionResult> Agent(Employee model)
        {
            var Employees = await _db.Employees.Where(e => e.IsActive).ToListAsync();
            return View(Employees);
        }

        [HttpGet]
        public IActionResult AddAgent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAgent(AgentVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (!await _roleManager.RoleExistsAsync("Agent"))
                return View(model);
            
            var user = new AppUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FirstName + " " + model.LastName,
                Email = model.Email,
                PhoneNumber = model.Phone,
                IsActive = true
            };
            if (user == null)
                return View(model);

            var result = await _userManager.CreateAsync(user,model.Password);

            if(result.Succeeded)
            {
                var Agent = new Employee
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FullName = model.FirstName + " " + model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Salary = model.Salary,
                    IsActive = true
                };

                if (model.Img != null)
                {
                    string fileName = Guid.NewGuid().ToString() +
                                      Path.GetExtension(model.Img.FileName);

                    string folderPath = Path.Combine(
                        _environment.WebRootPath,
                        "assets",
                        "images",
                        "employees");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Img.CopyToAsync(stream);
                    }

                   
                    Agent.Img = "/assets/images/employees/" + fileName;
                }
                var role = await _userManager.AddToRoleAsync(user, "Agent");

                await _db.Employees.AddAsync(Agent);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Agent));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", "");
            }

            return View(model);
        }

        public async Task<IActionResult> ViewAgent(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> EditAgent(int editagent)
        {
            var agent = await _db.Employees.FindAsync(editagent);

            if (agent == null)
                return NotFound();

            var model = new AgentVM
            {
                Id = agent.Id,
                FirstName = agent.FirstName,
                LastName = agent.LastName,
                Email = agent.Email,
                Phone = agent.Phone,
                CurrentImg = agent.Img
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAgent(AgentVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }
                

            var agent = await _db.Employees.FindAsync(model.Id);
            if (agent == null)
                return View(model);

            agent.FirstName = model.FirstName;
            agent.LastName = model.LastName;
            agent.Email = model.Email;
            agent.Phone = model.Phone;
            agent.Salary = model.Salary;
            agent.IsActive = true;

            var user = await _userManager.FindByIdAsync(agent.UserId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.FullName = model.FirstName + " " + model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);

            if (model.Img != null)
            {
                
                if (!string.IsNullOrEmpty(agent.Img))
                {
                    string oldImagePath = Path.Combine(
                        _environment.WebRootPath,
                        agent.Img.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

              
                string fileName = Guid.NewGuid().ToString() +
                                  Path.GetExtension(model.Img.FileName);

              
                string folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "assets",
                    "images",
                    "employees");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Img.CopyToAsync(stream);
                }

                
                agent.Img = "/assets/images/employees/" + fileName;
            }

            if (result.Succeeded)
            {
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Agent));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteAgent(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            emp.IsActive = false;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Agent));
        }
    }
}
