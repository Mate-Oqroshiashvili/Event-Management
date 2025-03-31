import { Component, OnInit } from '@angular/core';
import { EventDto, EventService } from '../services/event/event.service';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-events',
    imports: [RouterModule],
    templateUrl: './events.component.html',
    styleUrl: './events.component.css'
})
export class EventsComponent implements OnInit {
  events: EventDto[] = [];

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.getPublishedEvents();
  }

  getPublishedEvents() {
    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.events = data.events;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched the events successfully!');
      },
    });
  }
}
