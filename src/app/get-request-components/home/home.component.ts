import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../services/event/event.service';
import { RouterModule } from '@angular/router';
import { UserService } from '../../services/user/user.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  upcoming: EventDto[] = [];
  popular: EventDto[] = [];
  reviewsResult: number = 0;
  isLoggedIn$: boolean = false;

  isUpcomingLoading: boolean = false;
  isPopularLoading: boolean = false;

  constructor(
    private eventService: EventService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.userService.isAuthenticated$.subscribe((loggedIn) => {
      this.isLoggedIn$ = loggedIn;
      if (this.isLoggedIn$) {
        this.getUpcomingEvents();
        this.getPopularEvents();
      }
    });
  }

  getUpcomingEvents() {
    this.isUpcomingLoading = true;

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
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.isUpcomingLoading = false;
        console.log('Upcoming events fetched successfully!');
      },
    });
  }

  getPopularEvents() {
    this.isPopularLoading = true;

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
        this.isPopularLoading = false;
        console.log('Popular events fetched successfully!');
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
}
