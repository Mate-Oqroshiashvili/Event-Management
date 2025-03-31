import { Injectable } from '@angular/core';
import { EventDto } from '../event/event.service';
import { TicketDto } from '../ticket/ticket.service';
import { PurchaseDto } from '../purchase/purchase.service';
import { UserDto } from '../user/user.service';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

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

  constructor(private http: HttpClient) {}
}
