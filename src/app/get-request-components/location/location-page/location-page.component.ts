import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
import { Role, UserService } from '../../../services/user/user.service';
import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-location-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './location-page.component.html',
  styleUrls: ['./location-page.component.css', './responsive.css'],
})
export class LocationPageComponent implements OnInit {
  organizerId: number = 0;
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

  role: string = '';
  userId: number = 0;

  isAlreadyAdded: boolean = false;

  constructor(
    private userService: UserService,
    private locationService: LocationService,
    private eventService: EventService,
    private organizerService: OrganizerService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const locationIdParam = data.get('locationId');
      if (locationIdParam) {
        this.locationId = +locationIdParam;
        this.getUserInfo();
        this.getLocationById();
        this.getEventsByLocationId();
        this.getOrganizersByLocationId();
        if (this.role == 'ORGANIZER') {
          this.getOrganizer();
        }
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

  getOrganizer() {
    this.organizerService.getOrganizerByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.organizerId = data.organizerDto.id;
        console.log(data);

        let location = data.organizerDto.locations.find(
          (x: LocationDto) => x.id == this.locationId
        );
        if (location) {
          this.isAlreadyAdded = true;
        } else {
          this.isAlreadyAdded = false;
        }
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  addLocationOnOrganizer() {
    let message = '';

    if (this.organizerId) {
      this.organizerService
        .addOrganizerOnSpecificLocation(this.organizerId, this.locationId)
        .subscribe({
          next: (data: any) => {
            message = data;
          },
          error: (err) => {
            message = err.error.Message;
            Swal.fire('Oops!', message, 'error');
            console.error(err);
          },
          complete: () => {
            Swal.fire('Success!', message, 'success');
          },
        });
    } else {
      message = 'Organizer Id not found!';
      Swal.fire('Oops!', message, 'error');
    }
  }

  deleteLocation() {
    let message = '';

    this.locationService.removeLocation(this.locationId).subscribe({
      next: (data: any) => {
        message = data.message;
      },
      error: (err) => {
        message = err.error.Message;
        Swal.fire('Oops!', message, 'error');
      },
      complete: () => {
        Swal.fire('Success', message, 'success');
        this.router.navigate(['/locations']);
      },
    });
  }

  alert() {
    Swal.fire(
      'Success!',
      'Location is already linked to organizer!',
      'success'
    );
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
    this.role = decoded.role;
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
