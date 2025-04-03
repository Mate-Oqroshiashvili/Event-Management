import { UserCreateDto } from '../../services/user/user.service';
import { Component } from '@angular/core';
import { UserService } from '../../services/user/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  userCreateDto: UserCreateDto = {
    name: '',
    email: '',
    phoneNumber: '',
    password: '',
    emailCode: '',
    phoneNumberCode: '',
  };
  response: boolean = false;
  data: any;

  constructor(private userService: UserService) {}

  sendVerificationCodes() {
    let phoneNumber = this.userCreateDto.phoneNumber.replace(/\s+/g, '');

    this.userService
      .sendVerificationCodes(this.userCreateDto.email, phoneNumber)
      .subscribe({
        next: (response: any) => {
          this.response = response.result;
        },
        error: (err) => {
          console.error(err);
        },
        complete: () => {
          console.log('Verification codes sent successfully!');
        },
      });
  }

  registerUser() {
    this.userService.registerUser(this.userCreateDto).subscribe({
      next: (data: any) => {
        this.data = data;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('User registered successfully!');
      },
    });
  }
}
