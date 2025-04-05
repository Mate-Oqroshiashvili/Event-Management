import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  Role,
  UserDto,
  UserService,
  UserType,
} from '../../../services/user/user.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-info',
  imports: [CommonModule],
  templateUrl: './user-info.component.html',
  styleUrl: './user-info.component.css',
})
export class UserInfoComponent implements OnInit {
  userId: number = 0;
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

  constructor(
    private userService: UserService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      const userIdParam = data.get('userId');
      if (userIdParam) {
        this.userId = +userIdParam;
        this.getUserById();
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

  getUserType(type: number): string {
    return UserType[type] ?? 'Unknown Type';
  }

  getUserRole(role: number): string {
    return Role[role] ?? 'Unknown Role';
  }
}
