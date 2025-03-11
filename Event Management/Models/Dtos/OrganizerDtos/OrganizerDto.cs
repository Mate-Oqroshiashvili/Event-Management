using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Models.Dtos.OrganizerDtos
{
    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto User { get; set; }
        public List<EventDto>? Events { get; set; }
        public List<LocationDto>? Locations { get; set; }

        public OrganizerDto()
        {
            
        }
    }
}
