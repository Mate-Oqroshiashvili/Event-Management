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
  styleUrls: ['./locations.component.css', './responsive.css'],
})
export class LocationsComponent implements OnInit {
  locations: LocationDto[] = [];
  role: string = '';

  isLoading: boolean = false;

  constructor(
    private userService: UserService,
    private locationService: LocationService
  ) {}

  ngOnInit(): void {
    this.getAllLocations();
    this.getUserInfo();
  }

  getAllLocations() {
    this.isLoading = true;

    this.locationService.getAllLocations().subscribe({
      next: (data: any) => {
        if (data.locations) {
          this.locations = data.locations;
        } else {
          this.locations = data;
        }
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.isLoading = false;
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
