using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoradoHome.Models
{
    public class SavedProperty
    {
        public int? ClientId { get; set; }
        [ValidateNever]
        public Client? Client { get; set; }

        public int? PropertyId { get; set; }
        [ValidateNever]
        public Property? Property { get; set; }
    }
}
