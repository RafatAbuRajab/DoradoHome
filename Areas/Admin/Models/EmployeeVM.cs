namespace DoradoHome.Areas.Admin.Models
{
    public class EmployeeVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Img { get; set; }
        public string? CurrentImg { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
