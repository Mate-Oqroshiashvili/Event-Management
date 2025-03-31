import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import {
  LocationDto,
  LocationService,
} from '../services/location/location.service';

@Component({
  selector: 'app-locations',
  imports: [RouterModule],
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.css',
})
export class LocationsComponent implements OnInit {
  locations: LocationDto[] = [];

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    this.getAllLocations();
  }

  getAllLocations() {
    this.locationService.getAllLocations().subscribe({
      next: (data: any) => {
        this.locations = data.locations;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Locations fetched successfully!');
      },
    });
  }
}
