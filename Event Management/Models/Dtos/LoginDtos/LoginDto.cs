namespace Event_Management.Models.Dtos.LoginDtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }

        public LoginDto()
        {

        }
    }
}
