import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UserDto } from '../user/user.service';
import { Observable } from 'rxjs';

export interface ReviewCreateDto {
  starCount: number;
  userId: number;
  eventId: number;
}

export interface ReviewUpdateDto {
  starCount: number;
}

export interface ReviewDto {
  id: number;
  starCount: number;
  userId: number;
  user: UserDto | undefined;
  eventId: number;
}

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllReviews(): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.apiUrl}Review/get-all-reviews`);
  }

  getReviewById(reviewId: number): Observable<ReviewDto> {
    return this.http.get<ReviewDto>(
      `${this.apiUrl}Review/get-review-by-id/${reviewId}`
    );
  }

  getReviewsByEventId(eventId: number): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(
      `${this.apiUrl}Review/get-reviews-by-event-id/${eventId}`
    );
  }

  getReviewsByUserId(userId: number): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(
      `${this.apiUrl}Review/get-reviews-by-user-id/${userId}`
    );
  }

  addReview(reviewCreateDto: ReviewCreateDto): Observable<ReviewDto> {
    const formData = new FormData();
    Object.keys(reviewCreateDto).forEach((key) => {
      formData.append(key, (reviewCreateDto as any)[key]);
    });
    return this.http.post<ReviewDto>(
      `${this.apiUrl}Review/add-review`,
      formData
    );
  }

  updateReview(
    reviewId: number,
    userId: number,
    reviewUpdateDto: ReviewUpdateDto
  ): Observable<string> {
    const formData = new FormData();
    Object.keys(reviewUpdateDto).forEach((key) => {
      formData.append(key, (reviewUpdateDto as any)[key]);
    });
    return this.http.put<string>(
      `${this.apiUrl}Review/update-review/${reviewId}&${userId}`,
      formData
    );
  }

  removeReview(reviewId: number, userId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Review/remove-review/${reviewId}&${userId}`
    );
  }
}
