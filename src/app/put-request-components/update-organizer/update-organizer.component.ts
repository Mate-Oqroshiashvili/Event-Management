import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  OrganizerDto,
  OrganizerService,
  OrganizerUpdateDto,
} from '../../services/organizer/organizer.service';
import { UserService } from '../../services/user/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-update-organizer',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-organizer.component.html',
  styleUrl: './update-organizer.component.css',
})
export class UpdateOrganizerComponent implements OnInit {
  organizerForm!: FormGroup;
  backendErrors: any = {};
  imageFile: File | undefined;
  imagePreview: string = '';
  userId: number = 0;
  organizerId: number = 0;
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

  constructor(
    private fb: FormBuilder,
    private organizerService: OrganizerService,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.organizerId = +params.get('organizerId')!;
      this.getUserInfo();
      this.initForm();
      this.getOrganizer();
    });
  }

  initForm() {
    this.organizerForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });
  }

  getOrganizer() {
    this.organizerService.getOrganizerById(this.organizerId).subscribe({
      next: (data: any) => {
        this.organizer = data.organizerDto;

        this.organizerForm.patchValue({
          name: this.organizer.name,
          description: this.organizer.description,
          address: this.organizer.address,
          city: this.organizer.city,
          country: this.organizer.country,
        });

        this.imagePreview = this.organizer.logoUrl;
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  updateOrganizer() {
    const dto: OrganizerUpdateDto = {
      ...this.organizerForm.value,
      logo: this.imageFile,
    };

    let message = '';

    this.organizerService.UpdateOrganizer(this.organizerId, dto).subscribe({
      next: (data: any) => {
        message = data.message;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
        if (err.status === 400 && err.error?.errors) {
          this.backendErrors = err.error.errors;
        }

        if (err.error.Message) {
          message = err.error.Message;
          Swal.fire('Oops!', message, 'error');
        }
      },
      complete: () => {
        Swal.fire('Success', message, 'success');
        this.router.navigate(['organizer-panel', this.organizerId]);
      },
    });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files?.[0]) {
      const file = target.files[0];
      this.imageFile = file;

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();
    if (token) {
      const decoded: any = jwtDecode(token);
      this.userId = decoded.nameid;
    }
  }
}
