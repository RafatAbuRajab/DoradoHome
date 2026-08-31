using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Areas.Identity.Models
{
    public class RegisterVM
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
