import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserService } from '../user/user.service';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ParticipantSocketService {
  private hubConnection!: signalR.HubConnection;

  private balaneSubject = new BehaviorSubject<number | null>(null);
  balance$ = this.balaneSubject.asObservable();

  constructor(private userService: UserService) {}

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}participantHub`, {
        accessTokenFactory: () => this.userService.getToken() || '',
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Participant Hub Connection started'))
      .catch((err) =>
        console.error('Error starting SignalR connection: ', err)
      );

    this.hubConnection.on('GetBalance', (data: number) => {
      this.balaneSubject.next(data);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
