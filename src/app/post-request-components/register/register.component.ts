import { Component } from '@angular/core';
import { UserService } from '../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CanComponentDeactivate } from '../../services/guards/register.guard';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements CanComponentDeactivate {
  registerForm: FormGroup;
  response = false;
  serverErrors: any = {};

  constructor(private fb: FormBuilder, private userService: UserService) {
    this.registerForm = this.fb.group({
      name: [''],
      email: [''],
      phoneNumber: [''],
      password: [''],
      emailCode: [''],
      phoneNumberCode: [''],
    });
  }

  sendVerificationCodes() {
    this.serverErrors = {};

    const phoneNumber =
      this.registerForm.value.phoneNumber?.replace(/\s+/g, '') || '';
    const email = this.registerForm.value.email || '';

    this.userService.sendVerificationCodes(email, phoneNumber).subscribe({
      next: (res: any) => {
        this.response = res.result;
      },
      error: (err) => {
        console.error(err);

        if (err.status === 400 && err.error && err.error.errors) {
          this.serverErrors = err.error.errors;
        } else if (err.error?.Message) {
          const messageParts = err.error.Message.split('-');

          // Initialize the serverErrors object
          this.serverErrors = {};

          // Check for email-related error
          if (
            messageParts.some((part: any) =>
              part.toLowerCase().includes('email')
            )
          ) {
            this.serverErrors.email = messageParts.filter((part: any) =>
              part.toLowerCase().includes('email')
            );
          }

          // Check for phone number-related error
          if (
            messageParts.some((part: any) =>
              part.toLowerCase().includes('phone')
            )
          ) {
            this.serverErrors.phoneNumber = messageParts.filter((part: any) =>
              part.toLowerCase().includes('phone')
            );
          }
        }

        console.log(this.serverErrors);
      },
      complete: () => {
        console.log('Verification codes sent successfully!');
      },
    });
  }

  registerUser() {
    this.serverErrors = {};
    this.userService.registerUser(this.registerForm.value).subscribe({
      next: (data) => {
        console.log('Registered!', data);
      },
      error: (err) => {
        console.error(err);
        if (err.error && err.error.errors) {
          this.serverErrors = err.error.errors;
        }
      },
    });
  }

  canDeactivate(): boolean {
    return !this.registerForm.dirty;
  }
}
