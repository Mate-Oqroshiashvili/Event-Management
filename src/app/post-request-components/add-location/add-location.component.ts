import {
  LocationCreateDto,
  LocationService,
} from './../../services/location/location.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-location',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-location.component.html',
  styleUrl: './add-location.component.css',
})
export class AddLocationComponent implements OnInit {
  locationCreateDto: LocationCreateDto = {
    name: '',
    address: '',
    city: '',
    state: '',
    country: '',
    postalCode: '',
    maxCapacity: 0,
    availableStaff: 0,
    description: '',
    image: undefined,
    isIndoor: false,
    isAccessible: false,
  };
  imageFile: File | undefined;
  imagePreview: string = '';

  constructor(
    private locationService: LocationService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  addLocation() {
    this.locationCreateDto.image = this.imageFile;

    this.locationService.addLocation(this.locationCreateDto).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.router.navigate(['/locations']);
      },
    });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      Array.from(target.files).forEach((file) => {
        this.imageFile = file;

        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.imagePreview = e.target.result;
        };
        reader.readAsDataURL(file);
      });

      target.value = '';
    }
  }

  removeImage(): void {
    this.imageFile = undefined;
    this.imagePreview = '';
  }
}
