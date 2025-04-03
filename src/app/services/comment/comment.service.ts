import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UserDto } from '../user/user.service';
import { Observable } from 'rxjs';

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
  user: UserDto;
  eventId: number;
}

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllComments(): Observable<{ comments: Comment[] }> {
    return this.http.get<{ comments: Comment[] }>(
      `${this.apiUrl}Comment/get-all-comments`
    );
  }

  getCommentById(commentId: number): Observable<{ comment: Comment }> {
    return this.http.get<{ comment: Comment }>(
      `${this.apiUrl}Comment/get-comment-by-id/${commentId}`
    );
  }

  getCommentsByEventId(eventId: number): Observable<{ comments: Comment[] }> {
    return this.http.get<{ comments: Comment[] }>(
      `${this.apiUrl}Comment/get-comments-by-event-id/${eventId}`
    );
  }

  getCommentsByUserId(userId: number): Observable<{ comments: Comment[] }> {
    return this.http.get<{ comments: Comment[] }>(
      `${this.apiUrl}Comment/get-comments-by-user-id/${userId}`
    );
  }

  addComment(commentData: CommentCreateDto): Observable<{ comment: Comment }> {
    const formData = new FormData();
    Object.keys(commentData).forEach((key) => {
      formData.append(key, (commentData as any)[key]);
    });
    return this.http.post<{ comment: Comment }>(
      `${this.apiUrl}Comment/add-comment`,
      formData
    );
  }

  updateComment(
    commentId: number,
    userId: number,
    commentData: CommentUpdateDto
  ): Observable<{ message: string }> {
    const formData = new FormData();
    Object.keys(commentData).forEach((key) => {
      formData.append(key, (commentData as any)[key]);
    });
    return this.http.put<{ message: string }>(
      `${this.apiUrl}Comment/update-comment/${commentId}&${userId}`,
      formData
    );
  }

  removeComment(
    commentId: number,
    userId: number
  ): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}Comment/remove-comment/${commentId}&${userId}`
    );
  }
}
