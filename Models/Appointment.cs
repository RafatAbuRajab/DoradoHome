using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Appointment Date Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Appointment Time Required")]
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public TimeOnly Time { get; set; }

        public int? ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public int? PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        public Property? Property { get; set; }

        [Required(ErrorMessage = "Appointment Status Required")]
        public AppointmentStatus Status { get; set; }

        [Required(ErrorMessage = "Appointment Type Required")]
        public AppointmentType appointmentType { get; set; }

        public string? Notes { get; set; }

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }

    public enum AppointmentType
    {
        InPersonTour = 1,
        VirtualTour = 2,
        PhoneCall = 3
    }

    public enum AppointmentStatus
    {
        Confirmed = 1,
        Pending = 2,
        Rejected = 3,
        Closed = 4,
        PendingConfirmation = 5
    }
}
