import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../services/event/event.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ImageService } from '../../services/image/image.service';

@Component({
  selector: 'app-update-images',
  imports: [CommonModule, FormsModule],
  templateUrl: './update-images.component.html',
  styleUrl: './update-images.component.css',
})
export class UpdateImagesComponent implements OnInit {
  eventImages: string[] = [];
  uploadedImages: File[] = [];
  uploadedPreviews: string[] = [];
  eventId: number = 0;

  constructor(
    private eventService: EventService,
    private imageService: ImageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const eventIdParam = data.get('eventId');
      if (eventIdParam) {
        this.eventId = +eventIdParam;
        this.getImagesfromEvent();
      } else {
        console.error('Event ID not found in route parameters');
      }
    });
  }

  getImagesfromEvent() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data: any) => {
        this.eventImages = data.event.images;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Existing images fetched successfully!');
      },
    });
  }

  onFileSelected(event: any) {
    if (event.target.files.length > 0) {
      const files = Array.from(event.target.files) as File[];

      this.uploadedImages.push(...files);

      files.forEach((file) => {
        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.uploadedPreviews.push(e.target.result);
        };
        reader.readAsDataURL(file);
      });
    }
  }

  updateImages() {
    const formData = new FormData();

    this.eventImages.forEach((image) => {
      formData.append('existingImages', image);
    });

    this.uploadedImages.forEach((image) => {
      formData.append('formFile', image);
    });

    this.imageService
      .updateEventImages(this.eventId, this.eventImages, this.uploadedImages)
      .subscribe({
        next: (data) => {
          this.getImagesfromEvent();
          this.uploadedImages = [];
          this.uploadedPreviews = [];
        },
        error: (err) => {
          console.error('Error updating images:', err);
        },
        complete: () => {
          this.router.navigate(['events/event', this.eventId]);
        },
      });
  }

  removeUploadedImage(index: number): void {
    this.uploadedImages.splice(index, 1);
    this.uploadedPreviews.splice(index, 1);
  }

  removeExistingImage(index: number): void {
    this.eventImages.splice(index, 1);
    console.log('Existing image removed, update backend if needed.');
  }
}
