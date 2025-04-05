import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { UserDto } from '../user/user.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LocationDto } from '../location/location.service';

export interface OrganizerCreateDto {
  description: string;
  logo: File | undefined;
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
  createdAt: Date | undefined;
  user: UserDto | null;
  locations: LocationDto[] | null;
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

  getOrganizerById(organizerId: number): Observable<OrganizerDto> {
    return this.http.get<OrganizerDto>(
      `${this.apiUrl}Organizer/get-organizer-by-id/${organizerId}`
    );
  }

  getOrganizersByLocationId(locationId: number): Observable<OrganizerDto[]> {
    return this.http.get<OrganizerDto[]>(
      `${this.apiUrl}Organizer/get-organizers-by-location-id/${locationId}`,
      {}
    );
  }

  getOrganizerByUserId(userId: number): Observable<OrganizerDto> {
    return this.http.get<OrganizerDto>(
      `${this.apiUrl}Organizer/get-organizer-by-user-id/${userId}`,
      {}
    );
  }

  registerUserAsOrganizer(
    organizerData: OrganizerCreateDto
  ): Observable<OrganizerDto> {
    const formData = new FormData();
    Object.keys(organizerData).forEach((key) => {
      formData.append(key, (organizerData as any)[key]);
    });
    return this.http.post<OrganizerDto>(
      `${this.apiUrl}Organizer/register-user-as-organizer`,
      formData
    );
  }

  sendVerificationCodesForOrganizer(organizerId: number): Observable<string> {
    return this.http.post<string>(
      `${this.apiUrl}Organizer/send-verification-codes-for-organizer/${organizerId}`,
      {}
    );
  }

  verifyOrganizer(
    organizerId: number,
    emailCode: string,
    smsCode: string
  ): Observable<string> {
    return this.http.patch<string>(
      `${this.apiUrl}Organizer/verify-organizer/${organizerId}`,
      { emailCode, smsCode }
    );
  }

  addOrganizerOnSpecificLocation(
    organizerId: number,
    locationId: number
  ): Observable<string> {
    return this.http.post<string>(
      `${this.apiUrl}Organizer/add-organizer-on-specific-location/${organizerId}&${locationId}`,
      {}
    );
  }

  UpdateOrganizer(
    organizerId: number,
    organizerData: OrganizerUpdateDto
  ): Observable<string> {
    const formData = new FormData();
    Object.keys(organizerData).forEach((key) => {
      formData.append(key, (organizerData as any)[key]);
    });
    return this.http.put<string>(
      `${this.apiUrl}Organizer/update-organizer/${organizerId}`,
      formData
    );
  }

  removeOrganizer(organizerId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Organizer/remove-organizer/${organizerId}`
    );
  }

  removeOrganizerFromSpecificLocation(
    organizerId: number,
    locationId: number
  ): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Organizer/remove-organizer-from-specific-location/${organizerId}&${locationId}`
    );
  }
}
