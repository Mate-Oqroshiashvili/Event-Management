import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { UserDto } from '../user/user.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface OrganizerCreateDto {
  description: string;
  logo: File;
  address: string;
  city: string;
  country: string;
  userId: number;
}

export interface OrganizerUpdateDto {
  name?: string;
  email?: string;
  phoneNumber?: string;
  description?: string;
  logo?: File;
  address?: string;
  city?: string;
  country?: string;
  isVerified?: boolean;
}

export interface OrganizerDto {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  description: string;
  logoUrl: string;
  address: string;
  city: string;
  country: string;
  isVerified: boolean;
  createdAt: Date;
  user: UserDto;
}

@Injectable({
  providedIn: 'root',
})
export class OrganizerService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllOrganizers(): Observable<OrganizerDto[]> {
    return this.http.get<OrganizerDto[]>(
      `${this.apiUrl}Organizer/get-all-organizers`
    );
  }
}
