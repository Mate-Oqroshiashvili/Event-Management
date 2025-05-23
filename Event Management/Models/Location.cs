﻿namespace Event_Management.Models
{
    public class Location
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }  // Venue Name (e.g., "City Convention Center")
        public string Address { get; set; }  // Street Address
        public string City { get; set; }  // City Name
        public string State { get; set; }  // State/Province
        public string Country { get; set; }  // Country Name
        public string PostalCode { get; set; }  // ZIP or Postal Code
        public int MaxCapacity { get; set; } // Max number of attendees the venue can hold
        public int RemainingCapacity { get; set; }  // Current number of attendees the venue can hold
        public int AvailableStaff { get; set; }  // Max number of employeed and available staff
        public int BookedStaff { get; set; }  // Max number of bookable staff
        public string Description { get; set; }  // Additional details about the venue
        public string ImageUrl { get; set; }  // URL to venue image (optional)
        public bool IsIndoor { get; set; }  // True for indoor, False for outdoor venues
        public bool IsAccessible { get; set; }  // True if it has accessibility features

        // Navigation Property
        public List<Event> Events { get; set; } = new List<Event>();  // List of events held at this location
        public List<Organizer> Organizers { get; set; } = new List<Organizer>(); // Direct Many-to-Many

        public Location()
        {
            
        }
    }
}
