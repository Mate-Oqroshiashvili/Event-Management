import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../user/user.service';
import { inject } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';

export const organizerGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const token = userService.getToken();

  if (!token) {
    return false;
  }

  const decoded: any = jwtDecode(token);
  const role = decoded.role;

  if (role == 'ORGANIZER') {
    return true;
  }
  Swal.fire('Oops!', 'Only Organizer Is Allowed Here!', 'error');
  router.navigate(['/']);
  return false;
};
