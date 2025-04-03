import { Injectable } from '@angular/core';
import { UserDto } from '../user/user.service';
import { TicketDto } from '../ticket/ticket.service';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PromoCodeDto } from '../promo-code/promo-code.service';
import { Observable } from 'rxjs';

export enum PurchaseStatus {
  PENDING = 1,
  COMPLETED = 2,
  REFUNDED = 3,
}

export interface TicketPurchaseRequest {
  ticketId: number;
  quantity: number;
}

export interface PurchaseCreateDto {
  tickets: TicketPurchaseRequest[];
  userId: number;
  promoCodeText?: string;
}

export interface PurchaseUpdateDto {
  status?: PurchaseStatus;
  ticketIds?: number[];
}

export interface PurchaseDto {
  id: number;
  totalAmount: number;
  purchaseDate: string;
  status: PurchaseStatus;
  user: UserDto;
  promoCode?: PromoCodeDto;
  tickets: TicketDto[];
}

@Injectable({
  providedIn: 'root',
})
export class PurchaseService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllPurchases(): Observable<PurchaseDto[]> {
    return this.http.get<PurchaseDto[]>(
      `${this.apiUrl}Purchase/get-all-purchases`
    );
  }

  getOnePurchaseById(purchaseId: number): Observable<PurchaseDto> {
    return this.http.get<PurchaseDto>(
      `${this.apiUrl}Purchase/get-one-purchase-by-id/${purchaseId}`
    );
  }

  getPurchasesByUserId(userId: number): Observable<PurchaseDto[]> {
    return this.http.get<PurchaseDto[]>(
      `${this.apiUrl}Purchase/get-purchases-by-user-id/${userId}`
    );
  }

  purchaseTicket(
    purchaseCreateDto: PurchaseCreateDto
  ): Observable<PurchaseDto> {
    return this.http.post<PurchaseDto>(
      `${this.apiUrl}Purchase/purchase-ticket`,
      purchaseCreateDto
    );
  }
}
