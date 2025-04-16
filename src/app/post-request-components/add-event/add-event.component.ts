import { UserService } from './../../services/user/user.service';
import {
  EventCategory,
  EventCreateDto,
  EventService,
} from './../../services/event/event.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import {
  OrganizerDto,
  OrganizerService,
} from '../../services/organizer/organizer.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-add-event',
  imports: [CommonModule, ReactiveFormsModule],
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

  eventForm!: FormGroup;
  backendErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private eventService: EventService,
    private organizerService: OrganizerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getUserInfo();
    this.getOrganizerByUserId();

    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      capacity: [null, [Validators.required, Validators.min(1)]],
      locationId: [0, Validators.required],
      category: [0, Validators.required],
    });
  }

  addEvent() {
    const formValues = this.eventForm.value;

    const dto: EventCreateDto = {
      ...formValues,
      organizerId: this.organizer.id,
      images: this.imageFiles,
    };

    this.eventService.addEvent(dto).subscribe({
      next: (data) => {
        this.router.navigate(['/events']);
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors) {
          this.backendErrors = err.error.errors;
        }
        console.error(err);
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

  getFieldErrors(field: string): string | null {
    if (this.backendErrors[field]) {
      const filteredErrors = this.backendErrors[field].filter(
        (msg) => !msg.toLowerCase().includes('field')
      );
      return filteredErrors.join('\n') || null;
    }
    return null;
  }
}
