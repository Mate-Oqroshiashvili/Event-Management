import { Component, OnInit } from '@angular/core';
import {
  OrganizerDto,
  OrganizerService,
} from '../services/organizer/organizer.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import {
  LocationDto,
  LocationService,
} from '../services/location/location.service';
import {
  EventDto,
  EventService,
  EventStatus,
} from '../services/event/event.service';

@Component({
  selector: 'app-organizer-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './organizer-page.component.html',
  styleUrl: './organizer-page.component.css',
})
export class OrganizerPageComponent implements OnInit {
  organizerId: number = 0;
  organizer: OrganizerDto = {
    id: 0,
    name: '',
    email: '',
    phoneNumber: '',
    description: '',
    logoUrl: '',
    address: '',
    city: '',
    country: '',
    isVerified: false,
    createdAt: undefined,
    user: null,
  };

  locations: LocationDto[] = [];
  events: EventDto[] = [];

  constructor(
    private organizerService: OrganizerService,
    private locationService: LocationService,
    private eventSerivce: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const organizerIdParam = data.get('organizerId');
      if (organizerIdParam) {
        this.organizerId = +organizerIdParam;
        this.getOrganizerById();
        this.getLocationsByOrganizerId();
        this.getEventsByOrganizerId();
      } else {
        console.error('Organizer ID not found in route parameters');
      }
    });
  }

  getOrganizerById() {
    this.organizerService.getOrganizerById(this.organizerId).subscribe({
      next: (data: any) => {
        this.organizer = data.organizerDto;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Organizer fetched successfully!');
      },
    });
  }

  getLocationsByOrganizerId() {
    this.locationService.getLocationsByOrganizerId(this.organizerId).subscribe({
      next: (data: any) => {
        this.locations = data.locations;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Locations fetched successfully!');
      },
    });
  }

  getEventsByOrganizerId() {
    this.eventSerivce.getEventsByOrganizerId(this.organizerId).subscribe({
      next: (data: any) => {
        this.events = data.events;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Locations fetched successfully!');
      },
    });
  }

  getEventStatus(status: number): string {
    return EventStatus[status] ?? 'Unknown Status';
  }
}
