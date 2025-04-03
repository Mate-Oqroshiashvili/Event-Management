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
  startDate: Date | null;
  endDate: Date | null;
  capacity: number;
  status: EventStatus;
  bookedStaff: number;
  images: string[];
  location: LocationDto | null;
  organizer: OrganizerDto | null;
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

  getDraftedEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.apiUrl}Event/get-drafted-events`);
  }

  getCompletedEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(
      `${this.apiUrl}Event/get-completed-events`
    );
  }

  getDeletedEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.apiUrl}Event/get-deleted-events`);
  }

  getEventById(eventId: number): Observable<EventDto> {
    return this.http.get<EventDto>(
      `${this.apiUrl}Event/get-event-by-id/${eventId}`,
      {}
    );
  }

  getEventsBySearchTerm(searchTerm: string): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(
      `${this.apiUrl}Event/get-events-by-search-term/${searchTerm}`,
      {}
    );
  }

  getEventsByOrganizerId(organizerId: number): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(
      `${this.apiUrl}Event/get-events-by-organizer-id/${organizerId}`,
      {}
    );
  }

  getEventsByLocationId(locationId: number): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(
      `${this.apiUrl}Event/get-events-by-location-id/${locationId}`,
      {}
    );
  }

  addEvent(eventData: EventCreateDto): Observable<EventDto> {
    const formData = new FormData();
    Object.keys(eventData).forEach((key) => {
      formData.append(key, (eventData as any)[key]);
    });
    return this.http.post<EventDto>(`${this.apiUrl}Event/add-event`, formData);
  }

  addSpeakerOrArtistOnEvent(
    eventId: number,
    userId: number
  ): Observable<EventDto> {
    return this.http.post<EventDto>(
      `${this.apiUrl}Event/add-speaker-or-artist-on-event/${eventId}&${userId}`,
      {}
    );
  }

  removeSpeakerOrArtistFromEvent(
    eventId: number,
    userId: number
  ): Observable<EventDto> {
    return this.http.delete<EventDto>(
      `${this.apiUrl}Event/remove-speaker-or-artist-from-event/${eventId}&${userId}`,
      {}
    );
  }

  updateEventById(
    eventId: number,
    eventData: EventUpdateDto
  ): Observable<string> {
    const formData = new FormData();
    Object.keys(eventData).forEach((key) => {
      formData.append(key, (eventData as any)[key]);
    });
    return this.http.put<string>(
      `${this.apiUrl}Event/update-event/${eventId}`,
      formData
    );
  }

  rescheduleEvent(eventId: number, newDate: Date): Observable<string> {
    return this.http.put<string>(
      `${this.apiUrl}Event/reschedule-event/${eventId}`,
      newDate
    );
  }

  publishEvent(eventId: number): Observable<string> {
    return this.http.patch<string>(
      `${this.apiUrl}Event/publish-event/${eventId}`,
      {}
    );
  }

  removeEvent(eventId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Event/remove-event/${eventId}`,
      {}
    );
  }
}
