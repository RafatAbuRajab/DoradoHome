using DoradoHome.Areas.Agent.Models;
using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Agent.Controllers
{
    [Area("Agent")]
    [Authorize(Roles = "Agent")]
    public class ListingController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public ListingController(UserManager<AppUser> userManager,
            AppDbContext db,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> MyListing()
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

            var AgentData = await _db.Properties.Where(x => x.EmployeeId == agent.Id).ToListAsync();
            
            return View(AgentData);
        }

        [HttpGet]
        public async Task<IActionResult> ListProp()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var agent = await _db.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (agent == null)
            {
                return NotFound();
            }

            PropertyVM model = new();
            model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToListAsync();


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListProp(PropertyVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if(user == null) 
            { 
                return NotFound();
            }

            var employee = await _db.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(employee == null)
            {
                return NotFound();
            }

            var property = new Property
            {
                Name = model.Name,
                PropertyType = model.PropertyType,
                OfferType = model.OfferType,
                Status = model.Status,
                Description = model.Description,
                BedRooms = model.BedRooms,
                BathRooms = model.BathRooms,
                Area = model.Area,
                YearBuilt = model.YearBuilt,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Price = model.Price,
                ListingDate = DateTime.Today,
                EmployeeId = employee.Id,
                IsSold = false,
                IsRented = false,
                IsActive = true
            };

            await _db.Properties.AddAsync(property);
            await _db.SaveChangesAsync();

            if (model.Img != null && model.Img.Count > 0)
            {
                string folderPath = Path.Combine(
                    _environment.WebRootPath,
                    "assets",
                    "images",
                    "properties");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                bool isFirstImage = true;
                foreach (var image in model.Img)
                {
                    string fileName = Guid.NewGuid().ToString()
                                      + Path.GetExtension(image.FileName);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var propertyImage = new PropertyImage
                    {
                        PropertyId = property.Id,
                        ImageUrl = "/assets/images/properties/" + fileName,
                        IsMain = isFirstImage
                    };

                    if (propertyImage.IsMain)
                    {
                        property.Img = propertyImage.ImageUrl;
                    }

                    await _db.PropertyImages.AddAsync(propertyImage);

                    isFirstImage = false;
                }
                await _db.SaveChangesAsync();
            }

            foreach (var amenityId in model.SelectedAmenities)
            {
                var propertyAmenity = new PropertyAmenity
                {
                    PropertyId = property.Id,
                    AmenityId = amenityId
                };

                await _db.PropertyAmenities.AddAsync(propertyAmenity);
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyListing));
        }

        [HttpGet]
        public async Task<IActionResult> RentProp(int id)
        {
            var property = await _db.Properties.FindAsync(id);
            if(property == null)
            {
                return NotFound();
            }

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

            var rental = new Rental
            {
                Employee = agent,
                EmployeeId = agent.Id,
                Property = property,
                PropertyId = property.Id
            };

            ViewBag.Client = new SelectList(_db.Clients, "Id", "FullName");
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentProp(Rental model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                ViewBag.Client = new SelectList(_db.Clients, "Id", "FullName");
                return View(model);
            }

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

            var rental = new Rental
            {
                PropertyId = model.PropertyId,
                ClientId = model.ClientId,
                Employee = agent,
                EmployeeId = agent.Id,
                LeaseStartDate = model.LeaseStartDate,
                LeaseEndDate = model.LeaseEndDate,
                MonthlyRentPrice = model.MonthlyRentPrice,
                SecurityDepositPrice = model.SecurityDepositPrice,
                Notes = model.Notes,
                LeaseStatus = model.LeaseStatus,
                IsRented = true
            };

            await _db.Rentals.AddAsync(rental);
            await _db.SaveChangesAsync();

            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == rental.PropertyId);
            if(property == null)
            {
                return NotFound();
            }
            property.ClientId = model.ClientId;
            property.IsRented = true;
            property.Status = PropertyStatus.Sold;

            _db.Properties.Update(property);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyListing));
        }
    }
}
