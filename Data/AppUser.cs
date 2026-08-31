using DoradoHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Data
{
    public class AppUser : IdentityUser
    {
        [StringLength(20)]
        [ValidateNever]
        public string? FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is Required")]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [StringLength(20)]
        public string LastName { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
