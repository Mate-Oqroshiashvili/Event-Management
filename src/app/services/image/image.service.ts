import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  updateUserProfilePicture(userId: number, image: File): Observable<string> {
    const formData = new FormData();
    formData.append('formFile', image);

    return this.http.post<string>(
      `${this.apiUrl}Image/update-user-profile-picture/${userId}`,
      formData,
      { responseType: 'text' as 'json' }
    );
  }

  updateOrganizerLogoImage(
    organizerId: number,
    image: File
  ): Observable<string> {
    return this.http.post<string>(
      `${this.apiUrl}Image/update-organizer-logo-image/${organizerId}`,
      image
    );
  }

  updateEventImages(
    eventId: number,
    existingImages: string[],
    formFiles: File[]
  ) {
    const formData = new FormData();

    existingImages.forEach((image) => {
      formData.append('existingImages', image);
    });

    formFiles.forEach((file) => {
      formData.append('formFile', file);
    });

    return this.http.post<{ message: string }>(
      `${this.apiUrl}Image/update-event-images/${eventId}`,
      formData
    );
  }
}
