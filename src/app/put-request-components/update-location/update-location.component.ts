import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  LocationCreateDto,
  LocationDto,
  LocationService,
  LocationUpdateDto,
} from '../../services/location/location.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-location',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-location.component.html',
  styleUrl: './update-location.component.css',
})
export class UpdateLocationComponent implements OnInit {
  locationId: number = 0;
  location: LocationDto = {
    id: 0,
    name: '',
    address: '',
    city: '',
    state: '',
    country: '',
    postalCode: '',
    maxCapacity: 0,
    remainingCapacity: 0,
    availableStaff: 0,
    bookedStaff: 0,
    description: '',
    imageUrl: '',
    isIndoor: false,
    isAccessible: false,
  };
  locationForm!: FormGroup;
  imageFile: File | undefined;
  imagePreview: string = '';
  serverErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private locationService: LocationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const locatonIdParam = +params.get('locationId')!;

      if (locatonIdParam) {
        this.locationId = locatonIdParam;
        this.formInit();
        this.getLocation();
      }
    });
  }

  formInit() {
    this.locationForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      country: ['', Validators.required],
      postalCode: ['', Validators.required],
      description: [''],
      isIndoor: [false],
    });
  }

  getLocation() {
    this.locationService.getLocationById(this.locationId).subscribe({
      next: (data: any) => {
        this.location = data.location;

        this.locationForm.patchValue({
          name: this.location.name,
          address: this.location.address,
          city: this.location.city,
          state: this.location.state,
          country: this.location.country,
          postalCode: this.location.postalCode,
          description: this.location.description,
          isIndoor: this.location.isIndoor,
        });

        this.imagePreview = this.location.imageUrl;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
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

  updateLocation(): void {
    const locationData = this.locationForm.value as LocationUpdateDto;
    locationData.image = this.imageFile;

    this.locationService
      .updateLocation(this.locationId, locationData)
      .subscribe({
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
