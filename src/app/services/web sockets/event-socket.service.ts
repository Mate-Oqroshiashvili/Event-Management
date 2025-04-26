import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class EventSocketService {
  private hubConnection!: signalR.HubConnection;

  private publishedEventTitleSubject = new BehaviorSubject<string | null>(null);
  publishedEventTitle$ = this.publishedEventTitleSubject.asObservable();

  private updatedEventTitleSubject = new BehaviorSubject<string | null>(null);
  updatedEventTitle$ = this.updatedEventTitleSubject.asObservable();

  private rescheduledEventTitleSubject = new BehaviorSubject<string | null>(
    null
  );
  rescheduledEventTitle$ = this.rescheduledEventTitleSubject.asObservable();

  private deletedEventTitleSubject = new BehaviorSubject<string | null>(null);
  deletedEventTitle$ = this.deletedEventTitleSubject.asObservable();

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}eventHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Event Hub Connection started'))
      .catch((err) =>
        console.error('Error starting SignalR connection: ', err)
      );

    this.hubConnection.on('EventCreated', (data: string) => {
      this.publishedEventTitleSubject.next(data);
    });

    this.hubConnection.on('EventUpdated', (data: string) => {
      this.updatedEventTitleSubject.next(data);
    });

    this.hubConnection.on('EventRescheduled', (data: string) => {
      this.rescheduledEventTitleSubject.next(data);
    });

    this.hubConnection.on('EventDeleted', (data: string) => {
      this.deletedEventTitleSubject.next(data);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
