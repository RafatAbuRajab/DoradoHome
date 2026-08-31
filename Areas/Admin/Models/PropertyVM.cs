using DoradoHome.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Areas.Admin.Models
{
    public class PropertyVM
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Property Name")]
        [StringLength(50, ErrorMessage = "Property Name Less Than 50 Character")]
        [Display(Name = "Property Name")]
        public string Name { get; set; }

        [Display(Name = "Property Type")]
        public PropertyType PropertyType { get; set; }

        [Display(Name = "Offer Type")]
        public OfferType OfferType { get; set; }

        [Display(Name = "Status")]
        public PropertyStatus Status { get; set; }

        [Required(ErrorMessage = "Enter Property Description")]
        [StringLength(300, ErrorMessage = "Property Description Less Than 300 Character")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter BedRooms Number")]
        [Range(1, 10)]
        [Display(Name = "Bedrooms")]
        public int BedRooms { get; set; }

        [Required(ErrorMessage = " Enter BathRooms Number")]
        [Range(1, 10)]
        [Display(Name = "Bathrooms")]
        public int BathRooms { get; set; }

        [Required(ErrorMessage = " Enter Area Sq/Ft")]
        [Display(Name = "Area")]
        public int Area { get; set; }

        [Required(ErrorMessage = " Enter Year Built Number")]
        [Display(Name = "Year Built")]
        public int YearBuilt { get; set; }

        [Required(ErrorMessage = "Enter Property Address")]
        [StringLength(100, ErrorMessage = "Property Address Less Than 100 Character")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Enter City")]
        [StringLength(30, ErrorMessage = "City Less Than 30 Character")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter State")]
        [StringLength(10, ErrorMessage = "State Less Than 10 Character")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Enter ZIP Code")]
        [Display(Name = "ZIP Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Enter Property Price")]
        [Precision(18, 2)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Precision(18, 2)]
        public decimal? PricePerSquare { get; set; }

        public List<IFormFile>? Img { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ListingDate { get; set; }

        public int? EmployeeId { get; set; }
        [ValidateNever]
        public List<SelectListItem> Employees { get; set; }

        [ValidateNever]
        public List<SelectListItem> PropertyAmenities { get; set; }
        public List<int> SelectedAmenities { get; set; }

        [DefaultValue(false)]
        public bool IsSold { get; set; } = false;

        [DefaultValue(false)]
        public bool IsRented { get; set; } = false;

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
