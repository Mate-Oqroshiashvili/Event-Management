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

  constructor(private organizerSevice: OrganizerService) {}

  ngOnInit(): void {
    this.getAllOrganizers();
  }

  getAllOrganizers() {
    this.organizerSevice.getAllOrganizers().subscribe({
      next: (data: any) => {
        this.organizers = data.organizerDtos;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Organizers fetched successfully!');
      },
    });
  }
}
