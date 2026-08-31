using DoradoHome.Areas.Agent.Models;
using DoradoHome.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
    public class SettingsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public SettingsController(AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Setting(string Userid)
        {
            var user = await _userManager.FindByIdAsync(Userid);

            if (user == null)
                return NotFound();

            var settings = new SettingAgentVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setting(SettingAgentVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || user.IsActive == false)
                return View(model);

            var emp = await _db.Employees.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.FullName = model.FirstName + " " + model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            emp.FirstName = model.FirstName;
            emp.LastName = model.LastName;
            emp.FullName = model.FirstName + " " + model.LastName;
            emp.Email = model.Email;
            emp.Phone = model.Phone;

            var result = await _userManager.UpdateAsync(user);

            if (model.Img != null)
            {

                if (!string.IsNullOrEmpty(emp.Img))
                {
                    string oldImagePath = Path.Combine(
                        _environment.WebRootPath,
                        emp.Img.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

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


                emp.Img = "/assets/images/employees/" + fileName;
            }

            if (result.Succeeded)
            {
                _db.Employees.Update(emp);
                await _db.SaveChangesAsync();
                return View(model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Security()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Security(ChangePasswordAgentVM model, string id)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return View();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("logout", "User", new { area = "Identity" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}
