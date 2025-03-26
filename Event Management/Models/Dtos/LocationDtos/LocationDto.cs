using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.OrganizerDtos;

namespace Event_Management.Models.Dtos.LocationDtos
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public int MaxCapacity { get; set; } // Max number of attendees the venue can hold
        public int RemainingCapacity { get; set; }
        public int AvailableStaff { get; set; }
        public int BookedStaff { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsIndoor { get; set; }
        public bool IsAccessible { get; set; }
        public List<EventDto> Events { get; set; }
        public List<OrganizerDto> Organizers { get; set; }

        public LocationDto()
        {
            
        }
    }
}
