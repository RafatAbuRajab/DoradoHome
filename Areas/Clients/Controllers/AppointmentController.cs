using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Clients.Controllers
{
    [Area("Clients")]
    [Authorize(Roles = "Client")]
    public class AppointmentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public AppointmentController(UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> MyAppointments()
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

            var AppointmentData = await _db.Appointments
                .Where(x => x.ClientId == client.Id)
                .Include(x => x.Property)
                .Include(x => x.Offers)
                .Include(x => x.Employee)
                .ToListAsync();
                
            return View(AppointmentData);
        }

        public async Task<IActionResult> OfferAccepted(int id)
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

            var offer = await _db.Offers.FindAsync(id);
            if(offer == null || offer.ClientId != client.Id)
            {
                return NotFound();
            }

            var appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == offer.AppointmentId);
            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == offer.PropertyId);
            var employee = await _db.Employees.FirstOrDefaultAsync(x => x.Id == offer.EmployeeId);
            if(appointment == null || property == null || employee == null)
            {
                return NotFound();
            }

            offer.Property = property;
            offer.Employee = employee;
            offer.Status = OfferStatus.Accepted;

            appointment.Status = AppointmentStatus.Confirmed;

            if(offer.OfferType == OfferType.ForSale)
            {
                property.Status = PropertyStatus.Sold;
                property.IsSold = true;
                property.IsRented = false;

                if(offer.SalePrice == null)
                {
                    return BadRequest();
                }

                var sale = new Sale
                {
                    Property = property,
                    PropertyId = property.Id,
                    Employee = employee,
                    EmployeeId = employee.Id,
                    Client = client,
                    ClientId = client.Id,
                    SaleDate = DateTime.Today
                };
                property.Price = offer.SalePrice.Value;
                property.ClientId = client.Id;

                await _db.Sales.AddAsync(sale);
                await _db.SaveChangesAsync();
                return View(offer);
            }
            else
            {
                property.Status = PropertyStatus.Sold;
                property.IsRented = true;
                property.IsSold = false;

                if(offer.LeaseStart == null || offer.LeaseTerm == null || offer.MonthlyRent == null)
                {
                    return BadRequest();
                }

                var rental = new Rental
                {
                    Property = property,
                    PropertyId = property.Id,
                    Client = client,
                    ClientId = client.Id,
                    Employee = employee,
                    EmployeeId = employee.Id,
                    LeaseStartDate = offer.LeaseStart.Value,
                    LeaseEndDate = offer.LeaseStart.Value.AddMonths(offer.LeaseTerm.Value),
                    MonthlyRentPrice = offer.MonthlyRent.Value,
                    SecurityDepositPrice = offer.MonthlyRent.Value / 10,
                    Notes = offer.Notes,
                    LeaseStatus = LeaseStatus.Active,
                    IsActive = true,
                    IsRented = true
                };
                property.ClientId = client.Id;

                await _db.Rentals.AddAsync(rental);
                await _db.SaveChangesAsync();
            }

                return View(offer);
        }

        public async Task<IActionResult> OfferRejected(int id)
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

            var offer = await _db.Offers.FindAsync(id);
            if(offer == null || offer.ClientId != client.Id)
            {
                return NotFound();
            }

            var appointmentData = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == offer.AppointmentId);
            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == offer.PropertyId);
            var employee = await _db.Employees.FirstOrDefaultAsync(x => x.Id == offer.EmployeeId);
            if(appointmentData == null || property == null || employee == null)
            {
                return NotFound();
            }

            offer.Property = property;
            offer.Employee = employee;
            offer.Status = OfferStatus.Declined;

            appointmentData.Status = AppointmentStatus.Rejected;
            await _db.SaveChangesAsync();

            return View(offer);
        }
    }
}
