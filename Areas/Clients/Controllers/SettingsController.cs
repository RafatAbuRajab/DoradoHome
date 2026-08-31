using DoradoHome.Areas.Clients.Models;
using DoradoHome.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoradoHome.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "Client")]
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

            var settings = new SettingClientsVM
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
        public async Task<IActionResult> Setting(SettingClientsVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || user.IsActive == false)
                return View(model);

            var client = await _db.Clients.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.FullName = model.FirstName + " " + model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            client.FirstName = model.FirstName;
            client.LastName = model.LastName;
            client.FullName = model.FirstName + " " + model.LastName;
            client.Email = model.Email;
            client.Phone = model.Phone;

            var result = await _userManager.UpdateAsync(user);

            if (model.Img != null)
            {

                if (!string.IsNullOrEmpty(client.Img))
                {
                    string oldImagePath = Path.Combine(
                        _environment.WebRootPath,
                        client.Img.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

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
                    "clients");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }


                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Img.CopyToAsync(stream);
                }


                client.Img = "/assets/images/clients/" + fileName;
            }

            if (result.Succeeded)
            {
                _db.Clients.Update(client);
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
        public async Task<IActionResult> Security(ChangePasswordClientsVM model, string id)
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
