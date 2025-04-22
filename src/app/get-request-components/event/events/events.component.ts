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
  styleUrls: ['./events.component.css', './responsive.css'],
})
export class EventsComponent implements OnInit {
  role: string = '';
  searchTerm: string = '';
  events: EventDto[] = [];
  filteredEvents: EventDto[] = [];
  reviewsResult: number = 0;
  selectedSortOption: string = '';
  selectedCategoryOption: string = '';

  isLoading: boolean = false;

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
    this.isLoading = true;

    this.eventService.getPublishedEvents().subscribe({
      next: (data: any) => {
        this.events = data.events;
        this.applyFiltersAndSort();
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.isLoading = false;
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

  applyFiltersAndSort() {
    let result = [...this.events];
    if (this.selectedCategoryOption) {
      const selectedCategoryEnumValue =
        EventCategory[
          this.selectedCategoryOption as keyof typeof EventCategory
        ];

      result = result.filter(
        (event) => event.category === selectedCategoryEnumValue
      );
    }

    switch (this.selectedSortOption) {
      case 'dateAsc':
        result.sort((a, b) => {
          const dateA = a.startDate ? new Date(a.startDate).getTime() : 0;
          const dateB = b.startDate ? new Date(b.startDate).getTime() : 0;
          return dateA - dateB;
        });
        break;

      case 'dateDesc':
        result.sort((a, b) => {
          const dateA = a.startDate ? new Date(a.startDate).getTime() : 0;
          const dateB = b.startDate ? new Date(b.startDate).getTime() : 0;
          return dateB - dateA;
        });
        break;

      case 'rating':
        result.sort((a, b) => {
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
        result.sort((a, b) => a.capacity - b.capacity);
        break;

      case 'capacityDesc':
        result.sort((a, b) => b.capacity - a.capacity);
        break;

      default:
        break;
    }

    this.filteredEvents = result;
  }

  onLogicChange() {
    this.applyFiltersAndSort();
  }
}
