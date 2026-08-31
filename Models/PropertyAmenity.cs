using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoradoHome.Models
{
    public class PropertyAmenity
    {
        public int? PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        [ValidateNever]
        public Property? Property { get; set; }

        public int? AmenityId { get; set; }

        [ForeignKey(nameof(AmenityId))]
        [ValidateNever]
        public Amenity? Amenity { get; set; }
    }
}
