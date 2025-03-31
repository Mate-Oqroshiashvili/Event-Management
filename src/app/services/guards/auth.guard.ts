import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../user/user.service';

export const authGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);

  if (userService.isLoggedIn()) {
    return true;
  } else {
    userService.logout();
  }
  return false;
};
