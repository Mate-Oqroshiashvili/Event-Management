import { UserService } from './../../services/user/user.service';
import {
  EventCategory,
  EventCreateDto,
  EventService,
} from './../../services/event/event.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  OrganizerDto,
  OrganizerService,
} from '../../services/organizer/organizer.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-add-event',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-event.component.html',
  styleUrl: './add-event.component.css',
})
export class AddEventComponent implements OnInit {
  userId: number = 0;
  eventCreateDto: EventCreateDto = {
    title: '',
    description: '',
    startDate: undefined,
    endDate: undefined,
    capacity: 0,
    locationId: 0,
    organizerId: 0,
    category: EventCategory.No_Category,
    images: [],
  };
  organizer: OrganizerDto = {
    id: 0,
    name: '',
    email: '',
    phoneNumber: '',
    description: '',
    logoUrl: '',
    address: '',
    city: '',
    country: '',
    isVerified: false,
    createdAt: undefined,
    user: null,
    locations: null,
  };
  imageFiles: File[] = [];
  imagePreviews: string[] = [];

  constructor(
    private userService: UserService,
    private eventService: EventService,
    private organizerService: OrganizerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getUserInfo();
    this.getOrganizerByUserId();
  }

  addEvent() {
    this.eventCreateDto.images = this.imageFiles;
    this.eventCreateDto.organizerId = this.organizer.id;

    console.log(this.eventCreateDto);

    this.eventService.addEvent(this.eventCreateDto).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.router.navigate(['/events']);
      },
    });
  }

  getOrganizerByUserId() {
    this.organizerService.getOrganizerByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.organizer = data.organizerDto;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Organizer fetched successfully!');
      },
    });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      Array.from(target.files).forEach((file) => {
        this.imageFiles.push(file);

        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.imagePreviews.push(e.target.result);
        };
        reader.readAsDataURL(file);
      });

      target.value = '';
    }
  }

  removeImage(index: number): void {
    this.imageFiles.splice(index, 1);
    this.imagePreviews.splice(index, 1);
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
  }
}
