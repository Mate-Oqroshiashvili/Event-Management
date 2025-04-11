import { UserService, UserUpdateDto } from './../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-update-user',
  imports: [CommonModule, FormsModule],
  templateUrl: './update-user.component.html',
  styleUrl: './update-user.component.css',
})
export class UpdateUserComponent implements OnInit {
  userId: number = 0;
  userUpdateDto: UserUpdateDto = {
    name: '',
    email: '',
    phoneNumber: '',
    profilePicture: undefined,
  };
  imageFile: File | undefined;
  imagePreview: string = '';

  constructor(
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      this.userId = +data.get('userId')!;
    });
    this.getUserById();
  }

  getUserById() {
    this.userService.getUserById(this.userId).subscribe({
      next: (data: any) => {
        this.userUpdateDto.name = data.userDto.name;
        this.userUpdateDto.email = data.userDto.email;
        this.userUpdateDto.phoneNumber = data.userDto.phoneNumber;
        this.userUpdateDto.profilePicture = data.userDto.profilePicture;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('User fetched successfully!');
      },
    });
  }

  updateUser() {
    this.userUpdateDto.profilePicture = this.imageFile;
    console.log(this.userUpdateDto);
    this.userService
      .updateUserInformation(this.userId, this.userUpdateDto)
      .subscribe({
        next: (data: any) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          this.router.navigate(['/profile', this.userId, 'user-information']);
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
}
