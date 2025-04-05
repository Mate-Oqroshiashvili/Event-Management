import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../services/user/user.service';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';
import { OrganizerService } from '../services/organizer/organizer.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLoggedIn$: boolean = false;
  userId: number = 0;
  organizerId: number = 0;
  role: string = '';

  constructor(
    private userService: UserService,
    private organizerService: OrganizerService
  ) {}

  ngOnInit(): void {
    this.userService.isAuthenticated$.subscribe((loggedIn) => {
      this.isLoggedIn$ = loggedIn;
      this.getUserInfo();
      if (this.isLoggedIn$ && this.role === 'ORGANIZER') this.getOrganizer();
    });
  }

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
    this.role = decoded.role;
  }

  private getOrganizer() {
    this.organizerService.getOrganizerByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.organizerId = data.organizerDto.id;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Fetched organizer successfully!');
      },
    });
  }

  logout() {
    this.userService.logout();
  }
}
