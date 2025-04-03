import { CommonModule } from '@angular/common';
import {
  EventDto,
  EventService,
  EventStatus,
} from '../../services/event/event.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import {
  TicketDto,
  TicketService,
  TicketType,
} from '../../services/ticket/ticket.service';

@Component({
  selector: 'app-event-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './event-page.component.html',
  styleUrl: './event-page.component.css',
})
export class EventPageComponent implements OnInit {
  eventId: number = 0;
  event: EventDto = {
    id: 0,
    title: '',
    description: '',
    startDate: null,
    endDate: null,
    capacity: 0,
    status: EventStatus.DRAFT,
    bookedStaff: 0,
    images: [],
    location: null,
    organizer: null,
    tickets: [],
    speakersAndArtists: [],
    reviews: [],
    comments: [],
  };
  tickets: TicketDto[] = [];

  constructor(
    private eventService: EventService,
    private ticketService: TicketService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      if (eventIdParam) {
        this.eventId = +eventIdParam;
        this.getEventById();
        this.getTicketsByEventId();
      } else {
        console.error('Event ID not found in route parameters');
      }
    });
  }

  getEventById() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.event = data.event;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Event fetched successfully!');
      },
    });
  }

  getTicketsByEventId() {
    this.ticketService.getTicketsByEventId(this.eventId).subscribe({
      next: (data: any) => {
        this.tickets = data.tickets;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Tickets fetched successfully!');
      },
    });
  }

  getEventStatus(status: number): string {
    return EventStatus[status] ?? 'Unknown Status';
  }

  getTicketStatus(status: number): string {
    return TicketType[status] ?? 'Unknown Status';
  }
}
