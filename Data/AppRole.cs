using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Data
{
    public class AppRole : IdentityRole
    {
        [Required(ErrorMessage = "Role Description Required")]
        [StringLength(100, ErrorMessage = "Description Less Than 100 Character")]
        public string Description { get; set; }
    }
}
