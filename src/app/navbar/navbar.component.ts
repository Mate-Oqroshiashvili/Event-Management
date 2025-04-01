import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../services/user/user.service';
import { CommonModule, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLoggedIn$: boolean = false;
  searchTerm: string = '';
  userId: number = 0;
  role: string = '';

  constructor(private userService: UserService, private router: Router) {
    this.userService.isAuthenticated$.subscribe((loggedIn) => {
      this.isLoggedIn$ = loggedIn;
      this.getUserInfo();
    });
  }

  ngOnInit(): void {}

  private getUserInfo(): void {
    const token = this.userService.getToken();

    if (!token) return;

    const decoded: any = jwtDecode(token);
    this.userId = decoded.nameid;
    this.role = decoded.role;
  }

  searchLogic(searchTerm: string) {
    this.router.navigate(['search-result', searchTerm]);
    searchTerm = '';
  }

  logout() {
    this.userService.logout();
  }
}
