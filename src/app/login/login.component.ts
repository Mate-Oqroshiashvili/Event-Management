import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginDto, UserService } from '../services/user/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginDto: LoginDto = {
    email: '',
    password: '',
  };

  response: any;

  constructor(private userService: UserService, private router: Router) {}

  loginUser() {
    this.userService.loginUser(this.loginDto).subscribe({
      next: (data: any) => {
        this.response = data;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.router.navigate(['']);
        console.log('User logged in successfully!');
      },
    });
  }
}
