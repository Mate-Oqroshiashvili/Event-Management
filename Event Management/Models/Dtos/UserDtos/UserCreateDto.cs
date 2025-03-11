using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.UserDtos
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public UserCreateDto()
        {
            
        }
    }
}
