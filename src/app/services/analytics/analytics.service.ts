import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminAnalyticsDto, UserAnalyticsDto } from '../user/user.service';
import {
  EventAnalyticsDto,
  EventAnalyticsRequestDto,
} from '../event/event.service';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserAnalytics(userId: number): Observable<UserAnalyticsDto> {
    return this.http.get<UserAnalyticsDto>(
      `${this.apiUrl}Analytics/get-user-analytics/${userId}`
    );
  }

  getAdminAnalytics(): Observable<AdminAnalyticsDto> {
    return this.http.get<AdminAnalyticsDto>(
      `${this.apiUrl}Analytics/get-admin-analytics`
    );
  }

  getAnalytics(
    reqeust: EventAnalyticsRequestDto
  ): Observable<EventAnalyticsDto> {
    return this.http.post<EventAnalyticsDto>(
      `${this.apiUrl}Analytics/get-analytics`,
      reqeust
    );
  }
}
