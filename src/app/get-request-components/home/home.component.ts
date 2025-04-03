import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { EventDto, EventService } from '../../services/event/event.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  upcoming: EventDto[] = [];
  popular: EventDto[] = [];

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.getUpcomingEvents();
    this.getPopularEvents();
  }

  getUpcomingEvents() {
    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.upcoming = data.events
          .filter((event: EventDto) => event.startDate !== null)
          .sort(
            (a: EventDto, b: EventDto) =>
              new Date(b.startDate!).getTime() -
              new Date(a.startDate!).getTime()
          )
          .slice(0, 3);
      },
    });
  }

  getPopularEvents() {
    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.popular = data.events
          .filter((event: EventDto) => event.tickets?.length > 0)
          .sort(
            (a: EventDto, b: EventDto) =>
              b.tickets.reduce(
                (sum, ticket) => sum + (ticket.purchases?.length || 0),
                0
              ) -
              a.tickets.reduce(
                (sum, ticket) => sum + (ticket.purchases?.length || 0),
                0
              )
          )
          .slice(0, 3);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Popular events fetched successfully!');
      },
    });
  }
}
