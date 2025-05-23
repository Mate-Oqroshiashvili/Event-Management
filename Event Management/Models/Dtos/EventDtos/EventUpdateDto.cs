﻿using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.EventDtos
{
    public class EventUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Capacity { get; set; }

        public EventUpdateDto()
        {

        }
    }
}
