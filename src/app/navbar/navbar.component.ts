import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../services/user/user.service';
import { CommonModule, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLoggedIn$: boolean = false;
  searchTerm: string = '';

  constructor(private userService: UserService, private router: Router) {
    this.userService.isAuthenticated$.subscribe((loggedIn) => {
      this.isLoggedIn$ = loggedIn;
    });
  }

  ngOnInit(): void {}

  searchLogic(searchTerm: string) {
    this.router.navigate(['search-result', searchTerm]);
    searchTerm = '';
  }

  logout() {
    this.userService.logout();
  }
}
