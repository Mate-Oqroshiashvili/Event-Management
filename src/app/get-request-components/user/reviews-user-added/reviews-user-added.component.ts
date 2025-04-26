import { Component, OnInit } from '@angular/core';
import {
  ReviewDto,
  ReviewService,
} from '../../../services/review/review.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reviews-user-added',
  imports: [],
  templateUrl: './reviews-user-added.component.html',
  styleUrl: './reviews-user-added.component.css',
})
export class ReviewsUserAddedComponent implements OnInit {
  userId: number = 0;
  reviews: ReviewDto[] = [];

  constructor(
    private reviewService: ReviewService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      this.userId = +data.get('userId')!;
      this.getReviewsByUserId();
    });
  }

  getReviewsByUserId() {
    this.reviewService.getReviewsByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.reviews = data.reviews;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Reviews fetched successfully!');
      },
    });
  }
}
