import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

export interface CommentCreateDto {
  commentContent: string;
  userId: number;
  eventId: number;
}

export interface CommentUpdateDto {
  commentContent: string;
}

export interface CommentDto {
  id: number;
  commentContent: string;
  createdAt: Date;
  userId: number;
  eventId: number;
}

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}
}
