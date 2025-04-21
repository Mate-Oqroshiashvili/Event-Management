import {
  EventAnalyticsDto,
  EventAnalyticsRequestDto,
} from './../../../services/event/event.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterModule,
} from '@angular/router';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../../services/event/event.service';
import Swal from 'sweetalert2';
import { filter } from 'rxjs';
import {
  OrganizerDto,
  OrganizerService,
} from '../../../services/organizer/organizer.service';
import { Role } from '../../../services/user/user.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-organizer-panel',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './organizer-panel.component.html',
  styleUrl: './organizer-panel.component.css',
})
export class OrganizerPanelComponent implements OnInit {
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
    locations: null,
  };
  result: EventDto[] = [];
  resultType: string = 'drafted';
  reviewsResult: number = 0;

  modalActive = false;

  isLoading: boolean = false;

  allAnalyticsData: EventAnalyticsDto[] = [];
  eventsForAnalytics: EventDto[] = [];

  analyticsSearchTerm: string = '';

  constructor(
    private organizerService: OrganizerService,
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.organizerId = +data.get('organizerId')!;
      this.getDrafted();
      this.getOrganizer();
    });

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const snapshot = this.router.routerState.snapshot.root;
        const findOutlet = (route: any): boolean =>
          !!route.children?.some(
            (child: any) => child.outlet === 'add-modal' || findOutlet(child)
          );

        this.modalActive = findOutlet(snapshot);
      });
  }

  getOrganizer() {
    this.organizerService.getOrganizerById(this.organizerId).subscribe({
      next: (data: any) => {
        this.organizer = data.organizerDto;
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  getDrafted() {
    this.isLoading = true;

    this.eventService.getDraftedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
        console.log(this.result);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.resultType = 'drafted';
        this.isLoading = false;
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getPublished() {
    this.isLoading = true;

    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.resultType = 'published';
        this.isLoading = false;
        console.log('Fetched published events successfully!');
      },
    });
  }

  getCompleted() {
    this.isLoading = true;

    this.eventService.getCompletedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );

        this.eventsForAnalytics = this.result;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.resultType = 'completed';
        this.isLoading = false;
        console.log('Fetched completed events successfully!');
      },
    });
  }

  getRemoved() {
    this.isLoading = true;

    this.eventService.getDeletedEvents().subscribe({
      next: (data: any) => {
        this.result = [];
        this.result = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.resultType = 'removed';
        this.isLoading = false;
        console.log('Fetched removed events successfully!');
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

  deleteEvent(eventId: number) {
    let message = '';

    this.eventService.removeEvent(eventId).subscribe({
      next: (data: any) => {
        message = data.message;
        console.log(data);
      },
      error: (err) => {
        message = err.error.Message;
        Swal.fire('Oops!', message, 'error');
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success!', message, 'success');
        this.getDrafted();
      },
    });
  }

  publishEvent(eventId: number) {
    let message = '';

    this.eventService.publishEvent(eventId).subscribe({
      next: (data: any) => {
        message = data.message;
        console.log(data);
      },
      error: (err) => {
        message = err.error.Message;
        Swal.fire('Oops!', message, 'error');
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success!', message, 'success');
        this.getDrafted();
      },
    });
  }

  getOrganizerAnalytics() {
    this.isLoading = true;
    this.result = [];

    this.eventService.getCompletedEvents().subscribe({
      next: (data: any) => {
        this.eventsForAnalytics = data.events.filter(
          (x: EventDto) => x.organizer?.id === this.organizerId
        );

        this.allAnalyticsData = [];
        let completed = 0;

        for (const ev of this.eventsForAnalytics) {
          const req = {
            EventId: ev.id,
            OrganizerId: this.organizerId,
          };

          this.eventService.getAnalytics(req).subscribe({
            next: (analytics: any) => {
              this.allAnalyticsData.push(analytics);
            },
            complete: () => {
              completed++;
              if (completed === this.eventsForAnalytics.length) {
                this.resultType = 'analytics';
                this.isLoading = false;
              }
            },
            error: (err) => {
              console.error(err);
              completed++;
            },
          });
        }
      },
    });
  }

  get filteredAnalyticsData(): EventAnalyticsDto[] {
    return this.allAnalyticsData.filter((e) =>
      e.title.toLowerCase().includes(this.analyticsSearchTerm.toLowerCase())
    );
  }

  generateTicketTypeGradientFromData(data: any): string {
    const total = data.totalTicketQuantity;

    if (total === 0) return '#e0e0e0';

    const basicPercent = (data.basicTicketCount / total) * 100;
    const vipPercent = (data.vipTicketCount / total) * 100;
    const earlyPercent = (data.earlyBirdTicketCount / total) * 100;

    const basicEnd = basicPercent;
    const vipEnd = basicEnd + vipPercent;
    const earlyEnd = vipEnd + earlyPercent;

    return `conic-gradient(
      #007bff 0% ${basicEnd}%,
      #ffc107 ${basicEnd}% ${vipEnd}%,
      #28a745 ${vipEnd}% ${earlyEnd}%,
      #e0e0e0 ${earlyEnd}% 100%
    )`;
  }
}
