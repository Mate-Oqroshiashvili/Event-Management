import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { EventDto } from '../event/event.service';
import { UserDto } from '../user/user.service';
import { PurchaseDto } from '../purchase/purchase.service';
import { ParticipantDto } from '../participant/participant.service';
import { Observable } from 'rxjs';

export enum TicketType {
  BASIC = 1,
  VIP = 2,
  EARLYBIRD = 3,
}

export enum TicketStatus {
  AVAILABLE = 1,
  SOLD_OUT = 2,
  CANCELED = 3,
}

export interface TicketCreateDto {
  eventId: number;
  type: TicketType;
  price: number;
  quantity: number;
}

export interface TicketUpdateDto {
  price?: number;
  quantity?: number;
  status?: TicketStatus;
}

export interface TicketDto {
  id: number;
  type: TicketType;
  price: number;
  quantity: number;
  status: TicketStatus;
  qrCodeData: string;
  qrCodeImageUrl: string;
  event: EventDto;
  users: UserDto[];
  purchases: PurchaseDto[];
  participants: ParticipantDto[];
}

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTicketsByEventId(eventId: number): Observable<TicketDto[]> {
    return this.http.get<TicketDto[]>(
      `${this.apiUrl}Ticket/get-tickets-by-event-id/${eventId}`,
      {}
    );
  }
}
