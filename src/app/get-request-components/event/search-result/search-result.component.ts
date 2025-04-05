import { Component, OnInit } from '@angular/core';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../../services/event/event.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-result',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './search-result.component.html',
  styleUrl: './search-result.component.css',
})
export class SearchResultComponent implements OnInit {
  searchTerm: string = '';
  events: EventDto[] = [];
  reviewsResult: number = 0;

  constructor(
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const seachTerm = data.get('searchTerm');
      if (seachTerm) {
        this.searchTerm = seachTerm;
        this.getFoundEvents();
      } else {
        console.error('Search term not found in route parameters');
      }
    });
  }

  getFoundEvents() {
    this.eventService.getEventsBySearchTerm(this.searchTerm).subscribe({
      next: (data: any) => {
        this.events = data.events;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched the events by search term successfully!');
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
  }

  searchLogic(searchTerm: string) {
    this.router.navigate(['/search-result', searchTerm]);
    searchTerm = '';
  }

  getCategory(category: number): string {
    let categoryText = EventCategory[category] ?? 'Unknown Status';
    let result = categoryText.replaceAll('_', ' ');
    return result;
  }
}
