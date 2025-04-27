import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../user/user.service';
import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';

export const adminGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const token = userService.getToken();

  if (!token) {
    return false;
  }

  const decoded: any = jwtDecode(token);
  const role = decoded.role;

  if (role == 'ADMINISTRATOR') {
    return true;
  }
  Swal.fire('Oops!', 'Only Admin Is Allowed Here!', 'error');
  router.navigate(['/']);
  return false;
};
