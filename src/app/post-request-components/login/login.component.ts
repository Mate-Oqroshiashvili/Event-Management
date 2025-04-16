import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  serverErrors: any = {};

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: [''],
      password: [''],
    });
  }

  loginUser() {
    this.serverErrors = {};
    this.userService.loginUser(this.loginForm.value).subscribe({
      next: (data: any) => {
        console.log('Login success:', data);
      },
      error: (err) => {
        console.error(err);
        if (err.status === 400 && err.error && err.error.errors) {
          this.serverErrors = err.error.errors;
        }
      },
      complete: () => {
        this.router.navigate(['']);
        console.log('User logged in successfully!');
      },
    });
  }
}
