namespace Event_Management.Models.Dtos.OrganizerDtos
{
    public class OrganizerCreateDto
    {
        public string Description { get; set; }
        public IFormFile Logo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }

        public OrganizerCreateDto()
        {
            
        }
    }
}
