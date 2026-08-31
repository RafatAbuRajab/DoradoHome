using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DoradoHome.Models
{
    public class ScheduleVM
    {
        public int PropertyId { get; set; }
        [ValidateNever]
        public Property? Property { get; set; }

        public int EmployeeId { get; set; }
        [ValidateNever]
        public Employee? Employee { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public AppointmentType appointmentType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeOnly Time { get; set; }
        public string? AdditionalMessage { get; set; }
    }
}
