namespace DoradoHome.Models
{
    public class PaginationVM
    {
        public List<Property> Properties { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
