using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; }

        public int? PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        [ValidateNever]
        public Property? Property { get; set; }

        public int? ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        [ValidateNever]
        public Client? Client { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        [ValidateNever]
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Lease Start Date Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Lease Start Date")]
        public DateTime LeaseStartDate { get; set; }

        [Required(ErrorMessage = "Lease End Date Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Lease End Date")]
        public DateTime LeaseEndDate { get; set; }

        [Required(ErrorMessage = "Monthly Rent Price Required")]
        [Precision(18,2)]
        [Display(Name = "Monthly Rent (USD)")]
        public decimal MonthlyRentPrice { get; set; }

        [Required(ErrorMessage = "Security Deposit Price Required")]
        [Precision(18, 2)]
        [Display(Name = "Security Deposit (USD)")]
        public decimal SecurityDepositPrice { get; set; }

        [StringLength(300,ErrorMessage = "Note Less Than 300 Character")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public LeaseStatus LeaseStatus { get; set; }

        [DefaultValue(false)]
        public bool IsRented { get; set; } = false;

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }

    public enum LeaseStatus
    {
        Active = 1,
        PendingSignature = 2,
        EndingSoon = 3,
        Ended = 4,
        Expired = 5
    }
}
