using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Property Name")]
        [StringLength(50,ErrorMessage = "Property Name Less Than 50 Character")]
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
        [Range(1,10)]
        [Display(Name = "Bedrooms")]
        public int BedRooms { get; set; }

        [Required(ErrorMessage = " Enter BathRooms Number")]
        [Range(1,10)]
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
        [Precision(18,2)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Precision(18, 2)]
        public decimal? PricePerSquare => Price / Area;

        public string? Img { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ListingDate { get; set; }

        [DefaultValue(false)]
        public bool IsSold { get; set; } = false;

        [DefaultValue(false)]
        public bool IsRented { get; set; } = false;

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public int? ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        [ValidateNever]
        public Client? Client { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public Employee? Employee { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
        public ICollection<SavedProperty> SavedProperties { get; set; } = new List<SavedProperty>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }

    public enum PropertyType
    {
        Apartment = 1,
        Villa = 2,
        House = 3,
        Land = 4,
        Office = 5,
        Shop = 6,
        Warehouse = 7,
        CommercialBuilding = 8,
        ResidentialBuilding = 9,
        Farm = 10,
        Chalet = 11,
        Hotel = 12,
        Factory = 13,
        Garage = 14
    }
    public enum OfferType
    {
        ForSale = 1,
        ForRent = 2
    }
    public enum PropertyStatus
    {
        Active = 1,
        Pending = 2,
        Sold = 3,
        Rejected = 4
    }

}
