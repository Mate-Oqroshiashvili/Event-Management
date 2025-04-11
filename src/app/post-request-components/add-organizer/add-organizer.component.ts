import {
  OrganizerCreateDto,
  OrganizerService,
} from './../../services/organizer/organizer.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginDto, UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-add-organizer',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-organizer.component.html',
  styleUrl: './add-organizer.component.css',
})
export class AddOrganizerComponent implements OnInit {
  userId: number = 0;
  organizerId: number = 0;
  organizerCreateDto: OrganizerCreateDto = {
    description: '',
    logo: undefined,
    address: '',
    city: '',
    country: '',
    userId: 0,
  };
  imageFile: File | undefined;
  imagePreview: string = '';
  added: boolean = false;
  emailCode: string = '';
  smsCode: string = '';

  constructor(
    private organizerService: OrganizerService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getUserInfo();
  }

  addOrganizer() {
    this.organizerCreateDto.logo = this.imageFile;
    this.organizerCreateDto.userId = this.userId;

    this.organizerService
      .registerUserAsOrganizer(this.organizerCreateDto)
      .subscribe({
        next: (data: any) => {
          this.organizerId = data.organizerDto.id;
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          this.added = true;
          this.sendVerificationCodes();
        },
      });
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

  verifyOrganizer() {
    console.log(this.emailCode);
    console.log(this.smsCode);

    this.organizerService
      .verifyOrganizer(this.organizerId, this.emailCode, this.smsCode)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
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

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
  }
}
