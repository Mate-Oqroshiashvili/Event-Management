import {
  OrganizerCreateDto,
  OrganizerService,
} from './../../services/organizer/organizer.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-add-organizer',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './add-organizer.component.html',
  styleUrl: './add-organizer.component.css',
})
export class AddOrganizerComponent implements OnInit {
  organizerForm!: FormGroup;
  backendErrors: any = {};
  imageFile: File | undefined;
  imagePreview: string = '';
  added: boolean = false;
  emailCode: string = '';
  smsCode: string = '';
  userId: number = 0;
  organizerId: number = 0;

  constructor(
    private fb: FormBuilder,
    private organizerService: OrganizerService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getUserInfo();
    this.initForm();
  }

  initForm() {
    this.organizerForm = this.fb.group({
      address: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  addOrganizer() {
    const dto: OrganizerCreateDto = {
      ...this.organizerForm.value,
      logo: this.imageFile,
      userId: this.userId,
    };

    this.organizerService.registerUserAsOrganizer(dto).subscribe({
      next: (data: any) => {
        this.organizerId = data.organizerDto.id;
      },
      error: (err) => {
        console.error(err);
        if (err.status === 400 && err.error?.errors) {
          this.backendErrors = err.error.errors;
        }
      },
      complete: () => {
        this.added = true;
        this.sendVerificationCodes();
      },
    });
  }

  verifyOrganizer() {
    this.organizerService
      .verifyOrganizer(this.organizerId, this.emailCode, this.smsCode)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
          alert('Verification failed. Please check your codes and try again.');
        },
        complete: () => {
          this.added = false;
          this.userService.logout();
          this.router.navigate(['/login']);
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

  removeImage(): void {
    this.imageFile = undefined;
    this.imagePreview = '';
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();
    if (token) {
      const decoded: any = jwtDecode(token);
      this.userId = decoded.nameid;
    }
  }

  sendVerificationCodes() {
    this.organizerService
      .sendVerificationCodesForOrganizer(this.organizerId)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
      });
  }
}
