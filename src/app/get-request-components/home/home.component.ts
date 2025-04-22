import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../services/event/event.service';
import { RouterModule } from '@angular/router';
import { UserService } from '../../services/user/user.service';
import { PromoCodeService } from '../../services/promo-code/promo-code.service';
import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';
import { CanComponentDeactivate } from '../../services/guards/register.guard';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css', './responsive.css'],
})
export class HomeComponent implements OnInit, CanComponentDeactivate {
  userId: number = 0;
  published: EventDto[] = [];
  upcoming: EventDto[] = [];
  popular: EventDto[] = [];
  reviewsResult: number = 0;
  isLoggedIn$: boolean = false;
  canDeactivateBool: boolean = true;

  isLoading: boolean = false;

  promoCode: any = null;
  countdown: number = 0;
  countdownDisplay: string = '';

  nextPromoAvailable: Date | null = null;
  nextPromoFormatted: string = '';

  private timerInterval: any;

  constructor(
    private eventService: EventService,
    private userService: UserService,
    private promoCodeService: PromoCodeService
  ) {}

  ngOnInit(): void {
    this.userService.isAuthenticated$.subscribe((loggedIn) => {
      this.isLoggedIn$ = loggedIn;
      if (this.isLoggedIn$) {
        this.getUserInfo();
        this.getPublished();
      }
    });

    this.promoCodeService.nextPromoAvailable$.subscribe((date) => {
      this.nextPromoAvailable = date;
      this.formatNextPromoDate();
    });
  }

  getPublished() {
    this.isLoading = true;

    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        console.log(data);
        this.published = data.events;

        this.upcoming = this.published
          .filter((event: EventDto) => event.startDate !== null)
          .sort(
            (a: EventDto, b: EventDto) =>
              new Date(b.startDate!).getTime() -
              new Date(a.startDate!).getTime()
          )
          .slice(0, 3);

        this.popular = this.published
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
        this.isLoading = false;
        console.log('Events fetched successfully!');
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

  getRandomPromoCode() {
    this.promoCodeService.getRandomPromoCode(this.userId).subscribe({
      next: (data: any) => {
        this.promoCode = data.promoCode;
        this.canDeactivateBool = false;
        console.log(data);
        this.startCountdown(300);

        // Set next available promo date (3 days later)
        const nextAvailableDate = new Date();
        nextAvailableDate.setDate(nextAvailableDate.getDate() + 3);
        this.promoCodeService.updateNextPromoAvailable(nextAvailableDate);
      },
      error: (err) => {
        console.error(err);
        Swal.fire('Oops!', err.error.Message, 'error');
      },
    });
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
  }

  startCountdown(seconds: number) {
    clearInterval(this.timerInterval); // Clear any existing timer first
    this.countdown = seconds;
    this.updateCountdownDisplay();

    this.timerInterval = setInterval(() => {
      if (this.countdown <= 0) {
        clearInterval(this.timerInterval);
        this.promoCode = null;
        this.canDeactivateBool = true;
        return;
      }
      this.countdown--;
      this.updateCountdownDisplay();
    }, 1000);
  }

  updateCountdownDisplay() {
    const minutes = Math.floor(this.countdown / 60);
    const seconds = this.countdown % 60;
    this.countdownDisplay = `${minutes}m ${
      seconds < 10 ? '0' : ''
    }${seconds}s remaining to save`;
  }

  formatNextPromoDate() {
    if (!this.nextPromoAvailable) return;

    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false,
    };
    this.nextPromoFormatted = this.nextPromoAvailable.toLocaleString(
      'en-US',
      options
    );
  }

  get isPromoButtonDisabled(): boolean {
    if (!this.nextPromoAvailable) return false; // Allow if there's no restriction
    return new Date() < this.nextPromoAvailable;
  }

  getCategory(category: number): string {
    let categoryText = EventCategory[category] ?? 'Unknown Status';
    let result = categoryText.replaceAll('_', ' ');
    return result;
  }

  canDeactivate(): boolean {
    return this.canDeactivateBool;
  }
}
