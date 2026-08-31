namespace DoradoHome.Areas.Agent.Models
{
    public class SettingAgentVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Img { get; set; }
        public string? CurrentImg { get; set; }
    }
}
