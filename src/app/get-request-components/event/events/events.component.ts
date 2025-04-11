import { UserService } from './../../../services/user/user.service';
import { Component, OnInit } from '@angular/core';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../../services/event/event.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-events',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css',
})
export class EventsComponent implements OnInit {
  role: string = '';
  searchTerm: string = '';
  events: EventDto[] = [];
  reviewsResult: number = 0;
  selectedSortOption: string = '';

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getPublishedEvents();
    this.getUserInfo();
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

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.role = decoded.role;
  }

  searchLogic(searchTerm: string) {
    this.router.navigate(['search-result', searchTerm]);
    searchTerm = '';
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

  onSortChange() {
    switch (this.selectedSortOption) {
      case 'dateAsc':
        this.events.sort((a, b) => {
          const dateA = a.startDate ? new Date(a.startDate).getTime() : 0;
          const dateB = b.startDate ? new Date(b.startDate).getTime() : 0;
          return dateA - dateB;
        });
        break;

      case 'dateDesc':
        this.events.sort((a, b) => {
          const dateA = a.startDate ? new Date(a.startDate).getTime() : 0;
          const dateB = b.startDate ? new Date(b.startDate).getTime() : 0;
          return dateB - dateA;
        });
        break;

      case 'rating':
        this.events.sort((a, b) => {
          const aRating =
            a.reviews?.reduce((sum, r) => sum + r.starCount, 0) /
              (a.reviews?.length || 1) || 0;
          const bRating =
            b.reviews?.reduce((sum, r) => sum + r.starCount, 0) /
              (b.reviews?.length || 1) || 0;
          return bRating - aRating;
        });
        break;

      case 'capacityAsc':
        this.events.sort((a, b) => a.capacity - b.capacity);
        break;

      case 'capacityDesc':
        this.events.sort((a, b) => b.capacity - a.capacity);
        break;

      default:
        this.getPublishedEvents();
        break;
    }
  }
}
