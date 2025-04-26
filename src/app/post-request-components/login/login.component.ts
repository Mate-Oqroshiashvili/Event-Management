import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user/user.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  serverErrors: any = {};
  errorMessage: string = '';

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
        Swal.fire('Success!', 'Logged in successfully!', 'success');
      },
      error: (err) => {
        console.error(err);
        if (err.status === 400 && err.error && err.error.errors) {
          this.serverErrors = err.error.errors;
        }
        if (err.error.Message) {
          this.errorMessage = err.error.Message;
        }
      },
      complete: () => {
        this.router.navigate(['']);
        console.log('User logged in successfully!');
      },
    });
  }
}
