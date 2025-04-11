import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LocationCreateDto {
  name: string;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  maxCapacity: number;
  availableStaff: number;
  description: string;
  image: File | undefined;
  isIndoor: boolean;
  isAccessible: boolean;
}

export interface LocationUpdateDto {
  name?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  maxCapacity?: number;
  remainingCapacity?: number;
  availableStaff: number;
  bookedStaff: number;
  description?: string;
  image?: File;
  isIndoor?: boolean;
  isAccessible?: boolean;
}

export interface LocationDto {
  id: number;
  name: string;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  maxCapacity: number;
  remainingCapacity: number;
  availableStaff: number;
  bookedStaff: number;
  description: string;
  imageUrl: string;
  isIndoor: boolean;
  isAccessible: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllLocations(): Observable<LocationDto[]> {
    return this.http.get<LocationDto[]>(
      `${this.apiUrl}Location/get-all-locations`
    );
  }

  getLocationById(locationId: number): Observable<LocationDto> {
    return this.http.get<LocationDto>(
      `${this.apiUrl}Location/get-location-by-id/${locationId}`
    );
  }

  getLocationsByOrganizerId(organizerId: number): Observable<LocationDto[]> {
    return this.http.get<LocationDto[]>(
      `${this.apiUrl}Location/get-locations-by-organizer-id/${organizerId}`,
      {}
    );
  }

  addLocation(locationData: LocationCreateDto): Observable<LocationDto> {
    const formData = new FormData();
    Object.keys(locationData).forEach((key) => {
      formData.append(key, (locationData as any)[key]);
    });
    return this.http.post<LocationDto>(
      `${this.apiUrl}Location/add-location`,
      formData
    );
  }

  updateLocation(
    locationId: number,
    locationData: LocationUpdateDto
  ): Observable<string> {
    const formData = new FormData();
    Object.keys(locationData).forEach((key) => {
      formData.append(key, (locationData as any)[key]);
    });
    return this.http.put<string>(
      `${this.apiUrl}Location/update-location/${locationId}`,
      formData
    );
  }

  removeLocation(locationId: number): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}Location/remove-location/${locationId}`
    );
  }
}
