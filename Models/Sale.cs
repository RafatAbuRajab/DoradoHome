using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class Sale
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

        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime SaleDate { get; set; } = DateTime.Now.Date;
    }
}
