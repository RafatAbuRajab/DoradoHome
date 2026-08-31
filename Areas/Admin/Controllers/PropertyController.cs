using DoradoHome.Areas.Admin.Models;
using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PropertyController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public PropertyController(AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        public async Task<IActionResult> Property()
        {
            var properties = await _db.Properties.Where(x => x.IsActive).ToListAsync();
            return View(properties);
        }

        [HttpGet]
        public async Task<IActionResult> AddProp()
        {
            PropertyVM model = new();
            model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToListAsync();
            model.Employees = await _db.Employees.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName
            }).ToListAsync();
            

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProp(PropertyVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("","");
                PropertyVM prop = new();
                model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
                model.Employees = await _db.Employees.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FirstName + " " + x.LastName
                }).ToListAsync();

                return View(model);
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
                EmployeeId = model.EmployeeId,
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

                    if(propertyImage.IsMain)
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

            return RedirectToAction(nameof(Property));
        }

        [HttpGet]
        public async Task<IActionResult> EditProp(int id)
        {
            var prop = await _db.Properties.FindAsync(id);

            PropertyVM model = new PropertyVM
            {
                Id = prop.Id,
                Name = prop.Name,
                PropertyType = prop.PropertyType,
                OfferType = prop.OfferType,
                Status = prop.Status,
                Description = prop.Description,
                BedRooms = prop.BedRooms,
                BathRooms = prop.BathRooms,
                Area = prop.Area,
                YearBuilt = prop.YearBuilt,
                Address = prop.Address,
                City = prop.City,
                State = prop.State,
                ZipCode = prop.ZipCode,
                Price = prop.Price,
                ListingDate = prop.ListingDate,
                EmployeeId = prop.EmployeeId,
                IsSold = false,
                IsRented = false,
                IsActive = true
            };
            model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProp(PropertyVM model)
        {
            if(!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var item in error.Value.Errors)
                    {
                        ModelState.AddModelError("", item.ErrorMessage);
                    }
                }
                model.PropertyAmenities = await _db.Amenities.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();

                return View(model);
            }

            var prop = await _db.Properties.FindAsync(model.Id);

            if (prop == null)
                return NotFound();

            prop.Name = model.Name;
            prop.PropertyType = model.PropertyType;
            prop.OfferType = model.OfferType;
            prop.Status = model.Status;
            prop.Description = model.Description;
            prop.BedRooms = model.BedRooms;
            prop.BathRooms = model.BathRooms;
            prop.Area = model.Area;
            prop.YearBuilt = model.YearBuilt;
            prop.Address = model.Address;
            prop.City = model.City;
            prop.State = model.State;
            prop.ZipCode = model.ZipCode;
            prop.Price = model.Price;
            prop.ListingDate = model.ListingDate;
            prop.IsSold = false;
            prop.IsRented = false;
            prop.IsActive = true;

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

                bool hasMainImage = await _db.PropertyImages
                .AnyAsync(x => x.PropertyId == prop.Id && x.IsMain);

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
                        PropertyId = prop.Id,
                        ImageUrl = "/assets/images/properties/" + fileName,
                        IsMain = !hasMainImage
                    };

                    await _db.PropertyImages.AddAsync(propertyImage);

                    hasMainImage = true;
                }

                await _db.SaveChangesAsync();
            }

            _db.Properties.Update(prop);

            var oldAmenities = await _db.PropertyAmenities
            .Where(x => x.PropertyId == prop.Id)
            .ToListAsync();
            _db.PropertyAmenities.RemoveRange(oldAmenities);

            foreach (var amenityId in model.SelectedAmenities)
            {
                await _db.PropertyAmenities.AddAsync(new PropertyAmenity
                {
                    PropertyId = prop.Id,
                    AmenityId = amenityId
                });
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Property));
        }

        [HttpGet]
        public async Task<IActionResult> ViewProp(int id)
        {
            var prop = await _db.Properties.FindAsync(id);

            return View(prop);
        }

        public async Task<IActionResult> DeleteProp(int delid)
        {
            var prop = await _db.Properties.FindAsync(delid);
            prop.IsActive = false;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Property));
        }
    }
}
