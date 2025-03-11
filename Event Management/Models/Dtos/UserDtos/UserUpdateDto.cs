using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public Role? Role { get; set; }

        public UserUpdateDto()
        {
            
        }
    }
}
