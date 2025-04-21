import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import {
  OrganizerDto,
  OrganizerService,
} from '../../../services/organizer/organizer.service';

@Component({
  selector: 'app-organizers',
  imports: [RouterModule],
  templateUrl: './organizers.component.html',
  styleUrl: './organizers.component.css',
})
export class OrganizersComponent implements OnInit {
  organizers: OrganizerDto[] = [];

  isLoading: boolean = false;

  constructor(private organizerSevice: OrganizerService) {}

  ngOnInit(): void {
    this.getAllOrganizers();
  }

  getAllOrganizers() {
    this.isLoading = true;

    this.organizerSevice.getAllOrganizers().subscribe({
      next: (data: any) => {
        this.organizers = data.organizerDtos;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.isLoading = false;
        console.log('Organizers fetched successfully!');
      },
    });
  }
}
