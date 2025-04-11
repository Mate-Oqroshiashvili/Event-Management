import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import {
  EventCategory,
  EventDto,
  EventService,
} from '../../../services/event/event.service';

@Component({
  selector: 'app-organizer-panel',
  imports: [CommonModule, RouterModule],
  templateUrl: './organizer-panel.component.html',
  styleUrl: './organizer-panel.component.css',
})
export class OrganizerPanelComponent implements OnInit {
  organizerId: number = 0;
  result: EventDto[] = [];
  resultType: string = 'drafted';
  reviewsResult: number = 0;

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.organizerId = +data.get('organizerId')!;
      this.getDrafted();
    });
  }

  getDrafted() {
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
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getPublished() {
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
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getCompleted() {
    this.eventService.getCompletedEvents().subscribe({
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
        this.resultType = 'completed';
        console.log('Fetched drafted events successfully!');
      },
    });
  }

  getRemoved() {
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
        console.log('Fetched drafted events successfully!');
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
