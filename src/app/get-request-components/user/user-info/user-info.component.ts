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
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';
import { ParticipantService } from '../../../services/participant/participant.service';

@Component({
  selector: 'app-user-info',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css', './responsive.css'],
})
export class UserInfoComponent implements OnInit {
  userId: number = 0;
  userRole: string = '';
  userType: string = '';
  isVerified: boolean = false;
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

  balanceToAdd: number = 0;

  activeBottomTab = 'user-analytics';

  addingBalance: boolean = false;

  constructor(
    private userService: UserService,
    private organizerService: OrganizerService,
    private participantService: ParticipantService,
    private imageService: ImageService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      const userIdParam = data.get('userId');
      if (userIdParam) {
        this.userId = +userIdParam;
        this.getUserById();
      } else {
        console.error('User ID not found in route parameters');
      }
    });

    this.route.children.forEach((child) => {
      child.url.subscribe((segments) => {
        const bottomSegment =
          child.outlet === 'bottom' ? segments[0]?.path : null;
        if (bottomSegment) this.activeBottomTab = bottomSegment;
      });
    });

    this.participantService.refund$.subscribe(() => {
      this.getUserById();
    });
  }

  getUserById() {
    this.userService.getUserById(this.userId).subscribe({
      next: (data: any) => {
        this.user = data.userDto;
        this.userRole = this.user.role.toString();
        if (this.userRole === '3') {
          this.getOrganizer();
        }
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

  updateAddingBalance() {
    this.addingBalance = true;
  }

  cancelAddBalance() {
    this.addingBalance = false;
  }

  addBalance() {
    let message = '';

    this.userService.addBalance(this.userId, this.balanceToAdd).subscribe({
      next: (data: any) => {
        message = data.message;
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        Swal.fire('Success!', message, 'success');
        this.addingBalance = false;
        this.balanceToAdd = 0;
        this.getUserById();
      },
    });
  }

  registerAsArtist() {
    this.userService.changeUserType(this.userId, UserType.ARTIST).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.userService.logout();
      },
    });
  }

  registerAsSpeaker() {
    this.userService.changeUserType(this.userId, UserType.SPEAKER).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.userService.logout();
      },
    });
  }

  getBackToBasic() {
    this.userService.changeUserType(this.userId, UserType.BASIC).subscribe({
      next: (data: any) => {
        console.log(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.userService.logout();
      },
    });
  }

  setActiveBottomTab(tab: string) {
    this.activeBottomTab = tab;
  }

  generateBottomLink(bottomTab: string): any[] {
    const outlets: any = { bottom: [bottomTab] };
    return [{ outlets }];
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
