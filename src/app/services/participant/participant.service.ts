import { Injectable } from '@angular/core';
import { EventDto } from '../event/event.service';
import { TicketDto, TicketType } from '../ticket/ticket.service';
import { PurchaseDto } from '../purchase/purchase.service';
import { UserDto } from '../user/user.service';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

export interface ParticipantCreateDto {
  eventId: number;
  userId: number;
  ticketId: number;
}

export interface ParticipantUpdateDto {
  attendance?: boolean;
}

export interface ParticipantDto {
  id: number;
  registrationDate: Date;
  attendance: boolean;
  isUsed: boolean;
  event: EventDto;
  ticket: TicketDto;
  purchase: PurchaseDto;
  user: UserDto;
}

@Injectable({
  providedIn: 'root',
})
export class ParticipantService {
  private apiUrl: string = environment.apiUrl;

  private refundSubject = new Subject<void>();
  refund$ = this.refundSubject.asObservable();

  constructor(private http: HttpClient) {}

  notifyRefundCompleted() {
    this.refundSubject.next();
  }

  getAllParticipants(): Observable<ParticipantDto[]> {
    return this.http.get<ParticipantDto[]>(
      `${this.apiUrl}Participant/get-all-participants`
    );
  }

  getParticipantById(participantId: number): Observable<ParticipantDto> {
    return this.http.get<ParticipantDto>(
      `${this.apiUrl}Participant/get-participant-by-id/${participantId}`
    );
  }

  getParticipantsByUserId(userId: number): Observable<ParticipantDto[]> {
    return this.http.get<ParticipantDto[]>(
      `${this.apiUrl}Participant/get-participants-by-user-id/${userId}`
    );
  }

  registerUserAsParticipant(
    participantData: ParticipantCreateDto
  ): Observable<ParticipantDto> {
    const formData = new FormData();
    Object.keys(participantData).forEach((key) => {
      formData.append(key, (participantData as any)[key]);
    });
    return this.http.post<ParticipantDto>(
      `${this.apiUrl}Participant/register-user-as-participant`,
      formData
    );
  }

  requestTheRefund(
    participantId: number,
    purchaseId: number
  ): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Participant/request-the-refund/${participantId}&${purchaseId}`
    );
  }
}
