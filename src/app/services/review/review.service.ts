import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UserDto } from '../user/user.service';

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
  user: UserDto;
  eventId: number;
}

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}
}
