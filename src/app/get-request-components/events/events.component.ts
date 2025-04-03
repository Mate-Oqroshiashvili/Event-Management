import { Component, OnInit } from '@angular/core';
import { EventDto, EventService } from '../../services/event/event.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-events',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css',
})
export class EventsComponent implements OnInit {
  searchTerm: string = '';
  events: EventDto[] = [];

  constructor(private eventService: EventService, private router: Router) {}

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

  searchLogic(searchTerm: string) {
    this.router.navigate(['search-result', searchTerm]);
    searchTerm = '';
  }
}
