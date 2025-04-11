import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Role, UserService } from '../../../services/user/user.service';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  userId: number = 0;
  role: string = '';
  isVerified: boolean = false;

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.userId = +data.get('userId')!;

      if (this.route.snapshot.url.length === 2) {
        this.router.navigate([`/profile/${this.userId}/user-information`]);
      }
    });

    this.getUserById();
  }

  getUserById() {
    this.userService.getUserById(this.userId).subscribe({
      next: (data: any) => {
        this.role = data.userDto.role;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('User fetched successfully!');
      },
    });
  }
}
