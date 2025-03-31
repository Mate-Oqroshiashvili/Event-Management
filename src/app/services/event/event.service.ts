import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { LocationDto } from '../location/location.service';
import { OrganizerDto } from '../organizer/organizer.service';
import { TicketDto } from '../ticket/ticket.service';
import { ReviewDto } from '../review/review.service';
import { UserDto } from '../user/user.service';
import { CommentDto } from '../comment/comment.service';
import { Observable } from 'rxjs';

export enum EventStatus {
  DRAFT = 1,
  PUBLISHED = 2,
  COMPLETED = 3,
  DELETED = 4,
}

export interface EventCreateDto {
  title: string;
  description: string;
  startDate: Date;
  endDate: Date;
  capacity: number;
  locationId: number;
  organizerId: number;
  images: File[];
}

export interface EventUpdateDto {
  title?: string;
  description?: string;
  startDate?: Date;
  endDate?: Date;
  capacity?: number;
  status?: EventStatus;
  locationId?: number;
  organizerId?: number;
  images: File[];
}

export interface EventDto {
  id: number;
  title: string;
  description: string;
  startDate: Date;
  endDate: Date;
  capacity: number;
  status: EventStatus;
  bookedStaff: number;
  images: string[];
  location: LocationDto;
  organizer: OrganizerDto;
  tickets: TicketDto[];
  speakersAndArtists: UserDto[];
  reviews: ReviewDto[];
  comments: CommentDto[];
}

@Injectable({
  providedIn: 'root',
})
export class EventService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPublishedEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(
      `${this.apiUrl}Event/get-published-events`
    );
  }
}
