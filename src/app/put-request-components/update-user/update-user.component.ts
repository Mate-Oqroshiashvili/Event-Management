import { UserService, UserUpdateDto } from './../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-update-user',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-user.component.html',
  styleUrl: './update-user.component.css',
})
export class UpdateUserComponent implements OnInit {
  userId: number = 0;
  form!: FormGroup;
  imageFile: File | undefined;
  imagePreview: string = '';
  backendErrors: { [key: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.userId = +data.get('userId')!;
    });

    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
    });

    this.getUserById();
  }

  getUserById() {
    this.userService.getUserById(this.userId).subscribe({
      next: (data: any) => {
        this.form.patchValue({
          name: data.userDto.name,
          email: data.userDto.email,
          phoneNumber: data.userDto.phoneNumber,
        });
        this.imagePreview = data.userDto.profilePicture || '';
      },
      error: (err) => console.error(err),
    });
  }

  updateUser() {
    const userUpdateDto: UserUpdateDto = {
      ...this.form.value,
      profilePicture: this.imageFile,
    };

    this.userService
      .updateUserInformation(this.userId, userUpdateDto)
      .subscribe({
        next: () => {
          this.router.navigate(['/profile', this.userId]);
        },
        error: (err) => {
          if (err.status === 400 && err.error?.errors) {
            this.backendErrors = err.error.errors;
          }
          console.error(err);
        },
        complete: () => {
          Swal.fire(
            'Success',
            'User information updated sucessfully!',
            'success'
          );
        },
      });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      this.imageFile = target.files[0];

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(this.imageFile);

      target.value = '';
    }
  }

  removeImage(): void {
    this.imageFile = undefined;
    this.imagePreview = '';
  }

  getErrorMessages(controlName: string): string[] {
    return this.backendErrors[controlName] || [];
  }
}
