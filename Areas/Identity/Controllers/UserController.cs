using DoradoHome.Areas.Identity.Models;
using DoradoHome.Data;
using DoradoHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DoradoHome.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext appDbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
            _configuration = configuration;
        }
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Incorrect or Account Not Active");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            
            if(result.IsNotAllowed)
            {
                ModelState.AddModelError("", "You're Not Allowed To Sign In!");
            }

            if(result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your Account Locked Wait 30 Minutes!");
            }

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Incorrect Email or Password!");
            }

            if(result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = new AppUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FirstName + " " + model.LastName,
                Email = model.Email,
                PhoneNumber = model.Phone
            };
            if (user == null)
                return View(model);
            var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)
            {
                var client = new Client
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FullName = model.FirstName + " " + model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    IsActive = true
                };

                await _appDbContext.AddAsync(client);
                await _userManager.AddToRoleAsync(user, "Client");
                await _appDbContext.SaveChangesAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("index", "home", new { area = "" });
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", "");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Data Incorrect");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError("", "Email Doesn't Exist");
                return View(model);
            }

            model.VerifyCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

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

            mail.To.Add(model.Email);

            mail.Subject = "DoradoHome - Password Reset Verification";

            mail.Body = $@"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Reset Verification</title>
                </head>
                
                <body style=""margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;"">
                
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                           style=""background-color:#f4f6f8; padding:40px 15px;"">
                
                        <tr>
                            <td align=""center"">
                
                                <!-- Main Card -->
                                <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                                       style=""
                                           max-width:600px;
                                           width:100%;
                                           background-color:#ffffff;
                                           border-radius:12px;
                                           overflow:hidden;
                                           box-shadow:0 4px 20px rgba(0,0,0,0.08);
                                       "">
                
                                    <!-- Header -->
                                    <tr>
                                        <td style=""
                                            background-color:#1f1f1f;
                                            padding:30px 30px 25px;
                                            text-align:center;
                                        "">
                
                                            <div style=""
                                                font-size:28px;
                                                font-weight:bold;
                                                color:#ffffff;
                                                letter-spacing:-0.5px;
                                            "">
                                                DoradoHome<span style=""color:#dc3545;"">.</span>
                                            </div>
                
                                            <div style=""
                                                margin-top:7px;
                                                font-size:11px;
                                                color:#aaaaaa;
                                                letter-spacing:2px;
                                                text-transform:uppercase;
                                            "">
                                                Real Estate Management
                                            </div>
                
                                        </td>
                                    </tr>
                
                
                                    <!-- Content -->
                                    <tr>
                                        <td style=""padding:40px 45px 35px;"">
                
                                            <!-- Small Label -->
                                            <div style=""
                                                font-size:12px;
                                                font-weight:bold;
                                                color:#dc3545;
                                                text-transform:uppercase;
                                                letter-spacing:1.5px;
                                                margin-bottom:12px;
                                            "">
                                                Password Recovery
                                            </div>
                
                                            <!-- Title -->
                                            <h1 style=""
                                                margin:0;
                                                color:#222222;
                                                font-size:28px;
                                                line-height:1.3;
                                            "">
                                                Verify Your Identity
                                            </h1>
                
                                            <!-- Description -->
                                            <p style=""
                                                margin:18px 0 0;
                                                color:#777777;
                                                font-size:15px;
                                                line-height:1.7;
                                            "">
                                                We received a request to reset your DoradoHome
                                                account password. Use the verification code below
                                                to continue.
                                            </p>
                
                
                                            <!-- Divider -->
                                            <div style=""
                                                height:1px;
                                                background-color:#eeeeee;
                                                margin:28px 0;
                                            ""></div>
                
                
                                            <!-- Verification Code Box -->
                                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                <tr>
                                                    <td align=""center""
                                                        style=""
                                                            background-color:#f8f9fa;
                                                            border-left:4px solid #dc3545;
                                                            padding:25px 15px;
                                                            border-radius:6px;
                                                        "">
                
                                                        <div style=""
                                                            font-size:12px;
                                                            color:#777777;
                                                            text-transform:uppercase;
                                                            letter-spacing:2px;
                                                            margin-bottom:12px;
                                                        "">
                                                            Your Verification Code
                                                        </div>
                
                                                        <div style=""
                                                            font-size:34px;
                                                            font-weight:bold;
                                                            letter-spacing:8px;
                                                            color:#1f1f1f;
                                                        "">
                                                            {model.VerifyCode}
                                                        </div>
                
                                                    </td>
                                                </tr>
                                            </table>
                
                
                                            <!-- Security Notice -->
                                            <p style=""
                                                margin:25px 0 0;
                                                color:#888888;
                                                font-size:13px;
                                                line-height:1.6;
                                                text-align:center;
                                            "">
                                                For your security, do not share this code with anyone.
                                                If you did not request a password reset, you can safely
                                                ignore this email.
                                            </p>
                
                                        </td>
                                    </tr>
                
                
                                    <!-- Footer -->
                                    <tr>
                                        <td style=""
                                            background-color:#1f1f1f;
                                            padding:25px 30px;
                                            text-align:center;
                                        "">
                
                                            <div style=""
                                                font-size:20px;
                                                font-weight:bold;
                                                color:#ffffff;
                                            "">
                                                DoradoHome<span style=""color:#dc3545;"">.</span>
                                            </div>
                
                                            <p style=""
                                                margin:8px 0 0;
                                                font-size:11px;
                                                color:#888888;
                                            "">
                                                Secure account verification
                                            </p>
                
                                            <p style=""
                                                margin:15px 0 0;
                                                font-size:11px;
                                                color:#666666;
                                            "">
                                                &copy; {DateTime.Now.Year} DoradoHome. All rights reserved.
                                            </p>
                
                                        </td>
                                    </tr>
                
                                </table>
                
                            </td>
                        </tr>
                
                    </table>
                
                </body>
                </html>";

            mail.IsBodyHtml = true;

            await smtp.SendMailAsync(mail);

            HttpContext.Session.SetString("VerifyCode", model.VerifyCode);
            HttpContext.Session.SetString("VerifyCodeExpiration", DateTime.UtcNow.AddMinutes(5).ToString());
            HttpContext.Session.SetString("ResetEmail", model.Email);
            return RedirectToAction(nameof(VerifyCode));
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCode(string code1,string code2,string code3,string code4,string code5,string code6)
        {
            var code = code1.ToUpper() + code2.ToUpper() + code3.ToUpper() + code4.ToUpper() + code5.ToUpper() + code6.ToUpper();

            var storedCode = HttpContext.Session.GetString("VerifyCode");
            
            if (string.IsNullOrEmpty(storedCode))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var expirationString = HttpContext.Session.GetString("VerifyCodeExpiration");

            if (string.IsNullOrEmpty(expirationString))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var expiration = DateTime.Parse(expirationString);

            if (DateTime.UtcNow > expiration)
            {
                HttpContext.Session.Remove("VerifyCode");
                HttpContext.Session.Remove("VerifyCodeExpiration");

                ModelState.AddModelError("", "Verification code has expired.");

                return View();
            }

            if (code == storedCode)
            {
                HttpContext.Session.SetString("CodeVerified", "true");
                return RedirectToAction(nameof(ResetPassword));
            }
            else
            {
                ModelState.AddModelError("", "Incorrect Code");
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var verified = HttpContext.Session.GetString("CodeVerified");

            if (verified != "true")
            {
                return RedirectToAction(nameof(VerifyCode));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var verified = HttpContext.Session.GetString("CodeVerified");

            if (verified != "true")
            {
                return RedirectToAction(nameof(VerifyCode));
            }

            var email = HttpContext.Session.GetString("ResetEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if(result.Succeeded)
            {
                HttpContext.Session.Remove("VerifyCode");
                HttpContext.Session.Remove("VerifyCodeExpiration");
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("CodeVerified");
                return RedirectToAction(nameof(Login));
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home", new { area = "" });
        }
    }
}
