using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public OfferType OfferType { get; set; }

        public int? PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        [ValidateNever]
        public Property? Property { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public Employee? Employee { get; set; }

        public int? ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        [ValidateNever]
        public Client? Client { get; set; }

        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        [ValidateNever]
        public Appointment? Appointment { get; set; }


        [Range(0.01, double.MaxValue,ErrorMessage = "Sale price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ClosingDate { get; set; }

        [Range(0.01, double.MaxValue,ErrorMessage = "Monthly rent must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyRent { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LeaseStart { get; set; }

        [Range(1, 120, ErrorMessage = "Lease term must be between 1 and 120 months.")]
        public int? LeaseTerm { get; set; }

        [StringLength(1000,ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        [Required]
        public OfferStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public enum Offertype
    {
        Sale = 1,
        Rental = 2
    }
    public enum OfferStatus
    {
        Pending = 1,
        Accepted = 2,
        Declined = 3
    }
}
