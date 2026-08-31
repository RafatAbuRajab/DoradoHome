using System.Diagnostics;
using System.Threading.Tasks;
using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace DoradoHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger,
            UserManager<AppUser> userManager,
            AppDbContext db,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 6;

            int totalProperties = await _db.Properties.CountAsync();

            int totalPages = (int)Math.Ceiling(
                (double)totalProperties / pageSize);

            var properties = await _db.Properties
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginationVM
            {
                Properties = properties,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> Buy(int page = 1)
        {
            int pageSize = 6;

            int totalProperties = await _db.Properties.Where(x => x.OfferType == OfferType.ForSale).CountAsync();

            int totalPages = (int)Math.Ceiling(
                (double)totalProperties / pageSize);

            var properties = await _db.Properties
                .Where(x => x.OfferType == OfferType.ForSale)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginationVM
            {
                Properties = properties,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> Rent(int page = 1)
        {
            int pageSize = 6;

            int totalProperties = await _db.Properties.Where(x => x.OfferType == OfferType.ForRent).CountAsync();

            int totalPages = (int)Math.Ceiling(
                (double)totalProperties / pageSize);

            var properties = await _db.Properties
                .Where(x => x.OfferType == OfferType.ForRent)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginationVM
            {
                Properties = properties,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> SaveProperty(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Identity" });
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (client == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var property = await _db.Properties.FindAsync(id);
            if (property == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var saved = new SavedProperty
            {
                ClientId = client.Id,
                PropertyId = property.Id
            };

            await _db.SavedProperties.AddAsync(saved);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> About()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("Agent");

            var userId = usersInRole.Select(x => x.Id).ToList();

            var agents = await _db.Employees
                .Where(x => userId.Contains(x.UserId))
                .ToListAsync();

            return View(agents);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];
            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);

            using var smtp = new SmtpClient(host, port);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                email,
                password
            );

            var mail = new MailMessage();

            mail.From = new MailAddress(email);

            mail.To.Add(email);

            mail.ReplyToList.Add(
                new MailAddress(model.Email)
            );

            mail.Subject = model.Subject;

            mail.Body = $@" <!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>New Contact Message</title> </head> <body style=' margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif; color:#333333; '> <!-- Main Container --> <table width='100%' cellpadding='0' cellspacing='0' border='0' style=' background-color:#f4f6f8; padding:40px 15px; '> <tr> <td align='center'> <!-- Email Card --> <table width='600' cellpadding='0' cellspacing='0' border='0' style=' max-width:600px; width:100%; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08); '> <!-- ================= HEADER ================= --> <tr> <td style=' background-color:#1f1f1f; padding:28px 35px; text-align:center; '> <!-- Logo --> <div style=' font-size:28px; font-weight:700; line-height:1.2; '> <span style='color:#ffffff;'> DoradoHome </span> <span style='color:#dc3545;'> . </span> </div> <div style=' margin-top:8px; font-size:12px; color:#aaaaaa; letter-spacing:1px; text-transform:uppercase; '> Real Estate Management </div> </td> </tr> <!-- ================= TITLE ================= --> <tr> <td style=' padding:35px 35px 20px 35px; '> <div style=' font-size:13px; color:#dc3545; font-weight:bold; text-transform:uppercase; letter-spacing:1px; margin-bottom:10px; '> New Contact Message </div> <h1 style=' margin:0; font-size:26px; line-height:1.3; color:#222222; font-weight:700; '> You received a new message </h1> <p style=' margin:12px 0 0 0; font-size:14px; line-height:1.6; color:#777777; '> Someone has contacted you through the DoradoHome website. </p> </td> </tr> <!-- ================= DIVIDER ================= --> <tr> <td style='padding:0 35px;'> <div style=' height:1px; background-color:#eeeeee; '></div> </td> </tr> <!-- ================= SENDER INFO ================= --> <tr> <td style='padding:25px 35px 10px 35px;'> <h2 style=' margin:0 0 18px 0; font-size:16px; color:#222222; '> Contact Information </h2> <!-- Name --> <table width='100%' cellpadding='0' cellspacing='0' border='0'> <tr> <td width='42' valign='top'> <div style=' width:34px; height:34px; line-height:34px; text-align:center; background-color:#f8e8ea; color:#dc3545; border-radius:50%; font-size:15px; font-weight:bold; '> N </div> </td> <td valign='top'> <div style=' font-size:11px; color:#999999; text-transform:uppercase; letter-spacing:.5px; margin-bottom:3px; '> Name </div> <div style=' font-size:14px; color:#333333; font-weight:600; '> {model.FullName} </div> </td> </tr> </table> <div style='height:15px;'></div> <!-- Email --> <table width='100%' cellpadding='0' cellspacing='0' border='0'> <tr> <td width='42' valign='top'> <div style=' width:34px; height:34px; line-height:34px; text-align:center; background-color:#f8e8ea; color:#dc3545; border-radius:50%; font-size:14px; font-weight:bold; '> @ </div> </td> <td valign='top'> <div style=' font-size:11px; color:#999999; text-transform:uppercase; letter-spacing:.5px; margin-bottom:3px; '> Email Address </div> <div style=' font-size:14px; color:#333333; font-weight:600; '> {model.Email} </div> </td> </tr> </table> <div style='height:15px;'></div> <!-- Subject --> <table width='100%' cellpadding='0' cellspacing='0' border='0'> <tr> <td width='42' valign='top'> <div style=' width:34px; height:34px; line-height:34px; text-align:center; background-color:#f8e8ea; color:#dc3545; border-radius:50%; font-size:13px; font-weight:bold; '> S </div> </td> <td valign='top'> <div style=' font-size:11px; color:#999999; text-transform:uppercase; letter-spacing:.5px; margin-bottom:3px; '> Subject </div> <div style=' font-size:14px; color:#333333; font-weight:600; '> {model.Subject} </div> </td> </tr> </table> </td> </tr> <!-- ================= MESSAGE ================= --> <tr> <td style=' padding:25px 35px 35px 35px; '> <h2 style=' margin:0 0 15px 0; font-size:16px; color:#222222; '> Message </h2> <div style=' background-color:#f8f9fa; border-left:4px solid #dc3545; border-radius:6px; padding:20px; font-size:14px; line-height:1.8; color:#555555; '> {model.Message} </div> </td> </tr> <!-- ================= REPLY BUTTON ================= --> <tr> <td align='center' style=' padding:0 35px 35px 35px; '> <a href='mailto:{model.Email}' style=' display:inline-block; background-color:#dc3545; color:#ffffff; text-decoration:none; font-size:14px; font-weight:bold; padding:13px 28px; border-radius:6px; '> Reply to {model.FullName} </a> </td> </tr> <!-- ================= FOOTER ================= --> <tr> <td style=' background-color:#1f1f1f; padding:25px 35px; text-align:center; '> <div style=' font-size:18px; font-weight:bold; color:#ffffff; '> DoradoHome<span style='color:#dc3545;'>.</span> </div> <div style=' margin-top:8px; font-size:11px; color:#888888; line-height:1.6; '> This message was sent from the DoradoHome contact form. </div> <div style=' margin-top:15px; font-size:10px; color:#666666; '> © {DateTime.Now.Year} DoradoHome. All rights reserved. </div> </td> </tr> </table> </td> </tr> </table> </body> </html> ";

            mail.IsBodyHtml = true;

            await smtp.SendMailAsync(mail);

            return View();
        }

        public async Task<IActionResult> propertyDetails(int id)
        {
            var property = await _db.Properties
                .Include(x => x.Employee)
                .Include(x => x.PropertyImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(property == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            return View(property);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ScheduleToBuy(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Identity" });
            }

            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == id);
            var employee = await _db.Employees.FirstOrDefaultAsync(x => property.EmployeeId == x.Id);
            if(property == null || employee == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var schedule = new ScheduleVM
            {
                Property = property,
                PropertyId = property.Id,
                Employee = employee,
                EmployeeId = employee.Id
            };

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ScheduleToBuy(ScheduleVM model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Identity" });
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(client == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var appointment = new Appointment
            {
                Date = model.Date,
                Time = model.Time,
                ClientId = client.Id,
                EmployeeId = model.EmployeeId,
                PropertyId = model.PropertyId,
                Status = AppointmentStatus.Pending,
                appointmentType = model.appointmentType,
                Notes = model.AdditionalMessage
            };

            await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(appointmentConfirmed), new { id = appointment.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ScheduleToRent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Identity" });
            }

            var property = await _db.Properties.FirstOrDefaultAsync(x => x.Id == id);
            var employee = await _db.Employees.FirstOrDefaultAsync(x => property.EmployeeId == x.Id);
            if (property == null || employee == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var schedule = new ScheduleVM
            {
                Property = property,
                PropertyId = property.Id,
                Employee = employee,
                EmployeeId = employee.Id
            };

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ScheduleToRent(ScheduleVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Identity" });
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (client == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var appointment = new Appointment
            {
                Date = model.Date,
                Time = model.Time,
                ClientId = client.Id,
                EmployeeId = model.EmployeeId,
                PropertyId = model.PropertyId,
                Status = AppointmentStatus.Pending,
                appointmentType = model.appointmentType,
                Notes = model.AdditionalMessage
            };

            await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(appointmentConfirmed), new { id = appointment.Id });
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> appointmentConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if(client == null)
            {
                return RedirectToAction(nameof(Error404));
            }

            var appointment = await _db.Appointments
                .Include(x => x.Property)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id && x.ClientId == client.Id);

            if (appointment == null)
            {
                return RedirectToAction(nameof(Error404));
            }


            return View(appointment);
        }

        public IActionResult Error403()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
