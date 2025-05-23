﻿using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.TicketDtos
{
    public class TicketDto
    {
        public int Id { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TicketStatus Status { get; set; }
        public string QRCodeData { get; set; }  // QR Code String
        public string QRCodeImageUrl { get; set; }  // Image URL
        public EventDto Event { get; set; }
        public List<UserDto> Users { get; set; }
        public List<PurchaseDto> Purchases { get; set; }
        public List<ParticipantDto> Participants { get; set; }

        public TicketDto()
        {
            
        }
    }
}
