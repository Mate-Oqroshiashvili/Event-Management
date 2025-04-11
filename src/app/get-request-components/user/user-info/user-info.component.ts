import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  Role,
  UserDto,
  UserService,
  UserType,
} from '../../../services/user/user.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { OrganizerService } from '../../../services/organizer/organizer.service';
import { ImageService } from '../../../services/image/image.service';

@Component({
  selector: 'app-user-info',
  imports: [CommonModule, RouterModule],
  templateUrl: './user-info.component.html',
  styleUrl: './user-info.component.css',
})
export class UserInfoComponent implements OnInit {
  userId: number = 0;
  userRole: string = '';
  user: UserDto = {
    id: 0,
    name: '',
    email: '',
    phoneNumber: '',
    role: Role.BASIC,
    userType: UserType.BASIC,
    balance: 0,
    codeExpiration: undefined,
    isLoggedIn: false,
    organizer: null,
    tickets: [],
    purchases: [],
    participants: [],
    reviews: [],
    comments: [],
    usedPromoCodes: [],
    profilePicture: '',
  };
  organizerId: number = 0;
  imageFile: File | undefined;
  imagePreview: string = '';

  constructor(
    private userService: UserService,
    private organizerService: OrganizerService,
    private imageService: ImageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      const userIdParam = data.get('userId');
      if (userIdParam) {
        this.userId = +userIdParam;
        this.getUserById();
        this.getOrganizer();
      } else {
        console.error('User ID not found in route parameters');
      }
    });
  }

  getUserById() {
    this.userService.getUserById(this.userId).subscribe({
      next: (data: any) => {
        this.user = data.userDto;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('User fetched successfully!');
      },
    });
  }

  getOrganizer() {
    this.organizerService.getOrganizerByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.organizerId = data.organizerDto.id;
        console.log(this.organizerId);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Organizer fetched successfully!');
      },
    });
  }

  removeAsOrganizer() {
    this.organizerService.removeOrganizer(this.organizerId).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.userService.logout();
        this.router.navigate(['/login']);
      },
    });
  }

  deleteUser() {
    this.userService.removeUser(this.userId).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('User removed successfully!');
        this.router.navigate(['/events']);
      },
    });
  }

  changeProfilePicture() {
    this.imageService
      .updateUserProfilePicture(this.userId, this.imageFile!)
      .subscribe({
        next: (data) => {
          console.log(data);
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          this.imageFile = undefined;
          this.imagePreview = '';
          this.getUserById();
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

  getUserType(type: number): string {
    return UserType[type] ?? 'Unknown Type';
  }

  getUserRole(role: number): string {
    return Role[role] ?? 'Unknown Role';
  }
}
