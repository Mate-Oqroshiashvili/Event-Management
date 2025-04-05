import {
  EventCategory,
  EventService,
  EventUpdateDto,
} from './../../services/event/event.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EventDto, EventStatus } from '../../services/event/event.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-update-event',
  imports: [CommonModule, FormsModule],
  templateUrl: './update-event.component.html',
  styleUrl: './update-event.component.css',
})
export class UpdateEventComponent implements OnInit {
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
    category: EventCategory.IT_And_Technologies,
  };
  eventUpdateDto: EventUpdateDto = {
    title: '',
    description: '',
    startDate: null,
    endDate: null,
    capacity: 0,
  };

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      if (eventIdParam) {
        this.eventId = +eventIdParam;
        this.getEvent();
      } else {
        console.error('Event ID not found in route parameters');
      }
    });
  }

  getEvent() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.event = data.event;

        this.eventUpdateDto = {
          title: this.event.title,
          description: this.event.description,
          startDate: this.event.startDate,
          endDate: this.event.endDate,
          capacity: this.event.capacity,
        };

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

  updateEvent() {
    this.eventService
      .updateEventById(this.eventId, this.eventUpdateDto)
      .subscribe({
        next: (data) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          this.router.navigate(['events/event', this.eventId]);
        },
      });
  }
}
