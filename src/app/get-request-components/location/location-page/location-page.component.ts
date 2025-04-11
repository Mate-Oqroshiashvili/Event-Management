import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import {
  LocationDto,
  LocationService,
} from '../../../services/location/location.service';
import {
  EventCategory,
  EventDto,
  EventService,
  EventStatus,
} from '../../../services/event/event.service';
import {
  OrganizerDto,
  OrganizerService,
} from '../../../services/organizer/organizer.service';

@Component({
  selector: 'app-location-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './location-page.component.html',
  styleUrl: './location-page.component.css',
})
export class LocationPageComponent implements OnInit {
  locationId: number = 0;
  location: LocationDto = {
    id: 0,
    name: '',
    address: '',
    city: '',
    state: '',
    country: '',
    postalCode: '',
    maxCapacity: 0,
    remainingCapacity: 0,
    availableStaff: 0,
    bookedStaff: 0,
    description: '',
    imageUrl: '',
    isIndoor: false,
    isAccessible: false,
  };
  events: EventDto[] = [];
  organizers: OrganizerDto[] = [];
  reviewsResult: number = 0;

  constructor(
    private locationService: LocationService,
    private eventService: EventService,
    private organizerService: OrganizerService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const locationIdParam = data.get('locationId');
      if (locationIdParam) {
        this.locationId = +locationIdParam;
        this.getLocationById();
        this.getEventsByLocationId();
        this.getOrganizersByLocationId();
      } else {
        console.error('Location ID not found in route parameters');
      }
    });
  }

  getLocationById() {
    this.locationService.getLocationById(this.locationId).subscribe({
      next: (data: any) => {
        this.location = data.location;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Location fetched successfully!');
      },
    });
  }

  getEventsByLocationId() {
    this.eventService.getEventsByLocationId(this.locationId).subscribe({
      next: (data: any) => {
        const publishedEvents = data.events.filter(
          (event: EventDto) => event.status === EventStatus.PUBLISHED
        );
        this.events = publishedEvents;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Event fetched successfully!');
      },
    });
  }

  getOrganizersByLocationId() {
    this.organizerService.getOrganizersByLocationId(this.locationId).subscribe({
      next: (data: any) => {
        this.organizers = data.organizerDtos;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Organizers fetched successfully!');
      },
    });
  }

  getReviewResult(event: EventDto) {
    const reviews = event.reviews;

    if (reviews && reviews.length > 0) {
      const totalStars = reviews.reduce(
        (sum: number, review: any) => sum + review.starCount,
        0
      );
      this.reviewsResult = totalStars / reviews.length;
    } else {
      this.reviewsResult = 0;
    }

    return this.reviewsResult;
  }

  getCategory(category: number): string {
    let categoryText = EventCategory[category] ?? 'Unknown Status';
    let result = categoryText.replaceAll('_', ' ');
    return result;
  }

  getEventStatus(status: number): string {
    return EventStatus[status] ?? 'Unknown Status';
  }
}
