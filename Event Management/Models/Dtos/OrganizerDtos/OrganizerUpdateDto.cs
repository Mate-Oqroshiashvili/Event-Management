namespace Event_Management.Models.Dtos.OrganizerDtos
{
    public class OrganizerUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public IFormFile? Logo { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsVerified { get; set; }

        public OrganizerUpdateDto()
        {
            
        }
    }
}
