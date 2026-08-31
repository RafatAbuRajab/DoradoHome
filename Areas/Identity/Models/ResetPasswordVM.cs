using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Areas.Identity.Models
{
    public class ResetPasswordVM
    {
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
