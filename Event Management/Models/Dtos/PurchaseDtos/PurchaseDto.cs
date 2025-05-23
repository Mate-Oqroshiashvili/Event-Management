﻿using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PurchaseDtos
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public UserDto User { get; set; }
        public PromoCodeDto? PromoCode { get; set; }
        public List<TicketDto> Tickets { get; set; } = new();
        //public List<ParticipantDto> Participants { get; set; }
    }
}
