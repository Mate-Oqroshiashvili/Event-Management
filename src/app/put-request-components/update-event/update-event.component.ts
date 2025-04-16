import {
  EventCategory,
  EventService,
  EventUpdateDto,
} from './../../services/event/event.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { EventDto, EventStatus } from '../../services/event/event.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-update-event',
  imports: [CommonModule, ReactiveFormsModule],
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
  form!: FormGroup;
  backendErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      if (eventIdParam) {
        this.eventId = +eventIdParam;
        this.initializeForm();
        this.getEvent();
      } else {
        console.error('Event ID not found in route parameters');
      }
    });
  }

  initializeForm(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      capacity: [0, [Validators.required, Validators.min(1)]],
    });
  }

  getEvent() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.event = data.event;
        this.form.patchValue({
          title: this.event.title,
          description: this.event.description,
          startDate: this.formatDateForInput(this.event.startDate),
          endDate: this.formatDateForInput(this.event.endDate),
          capacity: this.event.capacity,
        });
      },
      error: (err) => console.error(err),
    });
  }

  updateEvent() {
    this.backendErrors = {};

    const updateDto: EventUpdateDto = this.form.value;

    this.eventService.updateEventById(this.eventId, updateDto).subscribe({
      next: () => {
        this.router.navigate(['events/event', this.eventId]);
      },
      error: (err) => {
        if (err.status === 400 && err.error?.errors) {
          this.backendErrors = err.error.errors;
        }
        console.error(err);
      },
    });
  }

  formatDateForInput(dateData: Date | string | null): string {
    if (!dateData) return '';

    const date = new Date(dateData);

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
