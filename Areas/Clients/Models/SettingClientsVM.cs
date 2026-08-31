namespace DoradoHome.Areas.Clients.Models
{
    public class SettingClientsVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Img { get; set; }
        public string? CurrentImg { get; set; }
    }
}
