import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import {
  LocationDto,
  LocationService,
} from '../../../services/location/location.service';
import { jwtDecode } from 'jwt-decode';
import { UserService } from '../../../services/user/user.service';

@Component({
  selector: 'app-locations',
  imports: [RouterModule],
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.css',
})
export class LocationsComponent implements OnInit {
  locations: LocationDto[] = [];
  role: string = '';

  constructor(
    private userService: UserService,
    private locationService: LocationService
  ) {}

  ngOnInit(): void {
    this.getAllLocations();
    this.getUserInfo();
  }

  getAllLocations() {
    this.locationService.getAllLocations().subscribe({
      next: (data: any) => {
        this.locations = data.locations;
        console.log(this.locations);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Locations fetched successfully!');
      },
    });
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.role = decoded.role;
  }
}
