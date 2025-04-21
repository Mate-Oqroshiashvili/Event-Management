import {
  LocationCreateDto,
  LocationService,
} from './../../services/location/location.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-location',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-location.component.html',
  styleUrl: './add-location.component.css',
})
export class AddLocationComponent implements OnInit {
  locationForm!: FormGroup;
  imageFile: File | undefined;
  imagePreview: string = '';
  serverErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private locationService: LocationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.locationForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      country: ['', Validators.required],
      postalCode: ['', Validators.required],
      maxCapacity: [0, [Validators.required, Validators.min(1)]],
      availableStaff: [0, [Validators.required, Validators.min(1)]],
      description: [''],
      isIndoor: [false],
      isAccessible: [false],
    });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files?.length) {
      this.imageFile = target.files[0];
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(this.imageFile);
    }
  }

  removeImage(): void {
    this.imageFile = undefined;
    this.imagePreview = '';
  }

  addLocation(): void {
    const locationData = this.locationForm.value as LocationCreateDto;
    locationData.image = this.imageFile;

    this.locationService.addLocation(locationData).subscribe({
      next: () => this.router.navigate(['/locations']),
      error: (errorResponse) => {
        console.error(errorResponse);
        if (errorResponse.status === 400 && errorResponse.error?.errors) {
          const validationErrors = errorResponse.error.errors;
          this.serverErrors = {};
          for (const field in validationErrors) {
            if (validationErrors.hasOwnProperty(field)) {
              this.serverErrors[field] = validationErrors[field];
            }
          }
        }
      },
    });
  }
}
