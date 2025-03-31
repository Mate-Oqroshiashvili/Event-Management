import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

export enum PromoCodeStatus {
  Available = 1,
  OutOfStock = 2,
}

export interface PromoCodeCreateDto {
  eventId: number;
  promoCodeText: string;
  saleAmountInPercentages: number;
  promoCodeAmount: number;
}

export interface PromoCodeUpdateDto {
  promoCodeAmount: number;
}

export interface PromoCodeDto {
  id: number;
  eventId: number;
  promoCodeText: string;
  saleAmountInPercentages: number;
  promoCodeStatus: PromoCodeStatus;
}

@Injectable({
  providedIn: 'root',
})
export class PromoCodeService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}
}
