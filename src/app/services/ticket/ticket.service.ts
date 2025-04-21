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
  type?: TicketType;
}

export interface TicketDto {
  id: number;
  type: TicketType;
  price: number;
  quantity: number;
  status: TicketStatus;
  qrCodeData: string;
  qrCodeImageUrl: string;
  event: EventDto | null;
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

  getAllTickets(): Observable<TicketDto[]> {
    return this.http.get<TicketDto[]>(`${this.apiUrl}Ticket/get-all-tickets`);
  }

  getTicketById(ticketId: number): Observable<TicketDto> {
    return this.http.get<TicketDto>(
      `${this.apiUrl}Ticket/get-ticket-by-id/${ticketId}`
    );
  }

  getTicketsByEventId(eventId: number): Observable<TicketDto[]> {
    return this.http.get<TicketDto[]>(
      `${this.apiUrl}Ticket/get-tickets-by-event-id/${eventId}`,
      {}
    );
  }

  getTicketsByUserId(userId: number): Observable<TicketDto[]> {
    return this.http.get<TicketDto[]>(
      `${this.apiUrl}Ticket/get-tickets-by-user-id/${userId}`
    );
  }

  addTicket(ticketCreateDto: TicketCreateDto): Observable<TicketDto> {
    const formData = new FormData();
    formData.append('eventId', ticketCreateDto.eventId.toString());
    formData.append('type', Number(ticketCreateDto.type).toString());
    formData.append('price', Number(ticketCreateDto.price).toString());
    formData.append('quantity', Number(ticketCreateDto.quantity).toString());
    return this.http.post<TicketDto>(
      `${this.apiUrl}Ticket/add-ticket`,
      formData
    );
  }

  validateTicket(qrCodeImage: File): Observable<string> {
    return this.http.post<string>(
      `${this.apiUrl}Ticket/validate-ticket`,
      qrCodeImage
    );
  }

  updateTicket(
    ticketId: number,
    ticketUpdateDto: TicketUpdateDto
  ): Observable<string> {
    const formData = new FormData();
    Object.keys(ticketUpdateDto).forEach((key) => {
      formData.append(key, (ticketUpdateDto as any)[key]);
    });
    return this.http.put<string>(
      `${this.apiUrl}Ticket/update-ticket/${ticketId}`,
      formData
    );
  }

  removeTicket(ticketId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Ticket/remove-ticket/${ticketId}`
    );
  }
}
