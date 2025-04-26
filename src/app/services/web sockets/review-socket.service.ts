import { Injectable } from '@angular/core';
import { Role, UserType } from '../user/user.service';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { ReviewDto } from '../review/review.service';

export interface userObject {
  id: number;
  profilePicture: string;
  name: string;
  role: Role;
  userType: UserType;
}

export interface Data {
  reviewDto: ReviewDto;
  userObject: userObject;
}

export interface DataUpdate {
  id: number;
  eventId: number;
  starCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class ReviewSocketService {
  private hubConnection!: signalR.HubConnection;

  private reviewSubject = new BehaviorSubject<Data | null>(null);
  review$ = this.reviewSubject.asObservable();

  private deleteReviewSubject = new BehaviorSubject<number | null>(null);
  deletedReview$ = this.deleteReviewSubject.asObservable();

  private updateReviewSubject = new BehaviorSubject<DataUpdate | null>(null);
  updatedReview$ = this.updateReviewSubject.asObservable();

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}reviewHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Review Hub Connection started'))
      .catch((err) =>
        console.error('Error starting SignalR connection: ', err)
      );

    this.hubConnection.on('ReceiveReview', (data: Data) => {
      this.reviewSubject.next(data);
    });

    this.hubConnection.on('ReceiveReviewIdForDeletion', (reviewId: number) => {
      this.deleteReviewSubject.next(reviewId);
    });

    this.hubConnection.on('ReceiveReviewForUpdate', (data: DataUpdate) => {
      this.updateReviewSubject.next(data);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
