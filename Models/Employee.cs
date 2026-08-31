using DoradoHome.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required(ErrorMessage = "Full Name Required")]
        [StringLength(15, ErrorMessage = "Full Name Less Than 15 Character")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "First Name Required")]
        [StringLength(15, ErrorMessage = "First Name Less Than 15 Character")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        [StringLength(15, ErrorMessage = "Last Name Less Than 15 Character")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email Required")]
        [StringLength(50)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Required")]
        [StringLength(15, MinimumLength = 10)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Salary Required")]
        [Precision(18, 2)]
        public decimal Salary { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime HireDate { get; set; }

        public string? Img { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
