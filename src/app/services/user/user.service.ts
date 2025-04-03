import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { OrganizerDto } from '../organizer/organizer.service';
import { TicketDto } from '../ticket/ticket.service';
import { ReviewDto } from '../review/review.service';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CommentDto } from '../comment/comment.service';
import { PurchaseDto } from '../purchase/purchase.service';
import { ParticipantDto } from '../participant/participant.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

export enum Role {
  BASIC = 1,
  PARTICIPANT = 2,
  ORGANIZER = 3,
  ADMINISTRATOR = 4,
}

export enum UserType {
  BASIC = 1,
  ARTIST = 2,
  SPEAKER = 3,
}

export interface UsedPromoCodeDto {
  id: number;
  promoCodeId: number;
  userId: number;
  usedDate: Date;
}

export interface UserCreateDto {
  name: string;
  email: string;
  phoneNumber: string;
  password: string;
  emailCode: string;
  phoneNumberCode: string;
}

export interface UserUpdateDto {
  name?: string;
  email?: string;
  phoneNumber?: string;
  profilePicture?: File;
  role?: Role;
  userType?: UserType;
}

export interface ChangePasswordDto {
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface UserDto {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  profilePictureUrl?: string;
  role: Role;
  userType: UserType;
  balance: number;
  emailVerificationCode?: string;
  smsVerificationCode?: string;
  codeExpiration: Date | undefined;
  isLoggedIn: boolean;
  organizer: OrganizerDto | null;
  tickets: TicketDto[];
  purchases: PurchaseDto[];
  participants: ParticipantDto[];
  reviews: ReviewDto[];
  comments: CommentDto[];
  usedPromoCodes: UsedPromoCodeDto[];
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl: string = environment.apiUrl;
  private tokenKey = 'authToken';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object,
    private router: Router
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.isAuthenticatedSubject.next(this.hasValidToken());
    }
  }

  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}User/get-all-users`);
  }

  getUserById(userId: number): Observable<UserDto> {
    return this.http.get<UserDto>(
      `${this.apiUrl}User/get-user-by-id/${userId}`
    );
  }

  getUserByEmail(email: string): Observable<UserDto> {
    return this.http.get<UserDto>(
      `${this.apiUrl}User/get-user-by-email/${email}`
    );
  }

  sendVerificationCodes(
    email: string,
    phoneNumber: string
  ): Observable<boolean> {
    const params = new HttpParams()
      .set('email', email)
      .set('phoneNumber', phoneNumber);

    return this.http.post<boolean>(`${this.apiUrl}User/send-codes`, null, {
      params,
    });
  }

  registerUser(userCreateDto: UserCreateDto): Observable<UserDto> {
    return this.http.post<UserDto>(
      `${this.apiUrl}User/register-user`,
      userCreateDto
    );
  }

  loginUser(loginDto: {
    email: string;
    password: string;
  }): Observable<{ token: string }> {
    return this.http
      .post<{ token: string }>(`${this.apiUrl}User/login-user`, loginDto)
      .pipe(
        tap((response) => {
          if (response.token) {
            this.storeToken(response.token);
          }
        })
      );
  }

  addBalance(userId: number, balance: number): Observable<string> {
    return this.http.patch<string>(`${this.apiUrl}User/add-balance/${userId}`, {
      balance,
    });
  }

  changeLoginStatus(userId: number): Observable<boolean> {
    return this.http.patch<boolean>(
      `${this.apiUrl}User/change-login-status/${userId}`,
      {}
    );
  }

  changeUserType(userId: number, userType: UserType): Observable<string> {
    return this.http.patch<string>(
      `${this.apiUrl}User/change-user-type/${userId}`,
      {
        userType,
      }
    );
  }

  changeUserPassword(
    userId: number,
    password: ChangePasswordDto
  ): Observable<string> {
    return this.http.patch<string>(
      `${this.apiUrl}User/change-user-password/${userId}`,
      {
        password,
      }
    );
  }

  updateUserInformation(
    userId: number,
    userData: UserUpdateDto
  ): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}User/update-user-information/${userId}`,
      userData
    );
  }

  removeUser(userId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}User/remove-user/${userId}`
    );
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded: any = jwtDecode(token);
      if (!decoded.exp) return false;

      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp > currentTime;
    } catch (error) {
      console.error('Invalid token:', error);
      return false;
    }
  }

  storeToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.isAuthenticatedSubject.next(true);
    const decoded: any = jwtDecode(token);
    localStorage.setItem('userId', decoded.nameid);
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.tokenKey);
    }
    return null;
  }

  isLoggedIn(): boolean {
    return this.hasValidToken();
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      let userId = localStorage.getItem('userId');

      if (!userId) {
        console.warn('No userId found in localStorage. Skipping API call.');
        this.clearAuthData();
        return;
      }

      this.http
        .patch<{ updated: boolean; message: string }>(
          `${this.apiUrl}User/change-login-status/${userId}`,
          {}
        )
        .subscribe({
          next: (response) => {
            console.log(response.message);
          },
          error: (error) => {
            console.error('Logout error:', error);
          },
          complete: () => {
            this.clearAuthData();
          },
        });
    }
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem('userId');
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['login']);
  }
}
