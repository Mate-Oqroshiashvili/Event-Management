namespace Event_Management.Models.Dtos.LocationDtos
{
    public class LocationUpdateDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public int? Capacity { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public bool? IsIndoor { get; set; }
        public bool? IsAccessible { get; set; }

        public LocationUpdateDto()
        {
            
        }
    }
}
