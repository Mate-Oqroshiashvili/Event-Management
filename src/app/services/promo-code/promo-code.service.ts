import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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

  getAllPromoCodes(): Observable<PromoCodeDto[]> {
    return this.http.get<PromoCodeDto[]>(
      `${this.apiUrl}PromoCode/get-all-promo-codes`
    );
  }

  getPromoCodeById(promoCodeId: number): Observable<PromoCodeDto> {
    return this.http.get<PromoCodeDto>(
      `${this.apiUrl}PromoCode/get-promo-code-by-id/${promoCodeId}`
    );
  }

  getPromoCodeBySearchTerm(searchTerm: string): Observable<PromoCodeDto> {
    return this.http.get<PromoCodeDto>(
      `${this.apiUrl}PromoCode/get-promo-code-by-search-term/${searchTerm}`
    );
  }

  getPromoCodesByEventId(eventId: number): Observable<PromoCodeDto[]> {
    return this.http.get<PromoCodeDto[]>(
      `${this.apiUrl}PromoCode/get-promo-codes-by-event-id/${eventId}`
    );
  }

  createPromoCode(
    promoCodeCreateDto: PromoCodeCreateDto
  ): Observable<PromoCodeDto> {
    return this.http.post<PromoCodeDto>(
      `${this.apiUrl}PromoCode/create-promo-code`,
      promoCodeCreateDto
    );
  }

  updatePromoCode(
    promoCodeId: number,
    promoCodeUpdateDto: PromoCodeUpdateDto
  ): Observable<string> {
    return this.http.put<string>(
      `${this.apiUrl}PromoCode/update-promo-code/${promoCodeId}`,
      promoCodeUpdateDto
    );
  }

  removePromoCode(promoCodeId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}PromoCode/remove-promo-code/${promoCodeId}`
    );
  }
}
