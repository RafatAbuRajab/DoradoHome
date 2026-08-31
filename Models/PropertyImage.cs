using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false;

        public int? PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        [ValidateNever]
        public Property? Property { get; set; }
    }
}
