import { CommentDto } from './../comment/comment.service';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { Role, UserType } from '../user/user.service';

export interface userObject {
  id: number;
  profilePicture: string;
  name: string;
  role: Role;
  userType: UserType;
}

export interface Data {
  commentDto: CommentDto;
  userObject: userObject;
}

export interface DataUpdate {
  id: number;
  eventId: number;
  commentContent: string;
}

@Injectable({
  providedIn: 'root',
})
export class CommentSocketService {
  private hubConnection!: signalR.HubConnection;

  private commentSubject = new BehaviorSubject<Data | null>(null);
  comment$ = this.commentSubject.asObservable();

  private deleteCommentSubject = new BehaviorSubject<number | null>(null);
  deletedComment$ = this.deleteCommentSubject.asObservable();

  private updateCommentSubject = new BehaviorSubject<DataUpdate | null>(null);
  updatedComment$ = this.updateCommentSubject.asObservable();

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}commentHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Comment Hub Connection started'))
      .catch((err) =>
        console.error('Error starting SignalR connection: ', err)
      );

    this.hubConnection.on('ReceiveComment', (data: Data) => {
      this.commentSubject.next(data);
    });

    this.hubConnection.on(
      'ReceiveCommentIdForDeletion',
      (commentId: number) => {
        this.deleteCommentSubject.next(commentId);
      }
    );

    this.hubConnection.on('ReceiveCommentForUpdate', (data: DataUpdate) => {
      this.updateCommentSubject.next(data);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
