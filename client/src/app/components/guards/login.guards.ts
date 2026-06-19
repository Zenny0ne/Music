import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../../services/user.service';

export const loginGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  if(userService.isLoggedIn()){
    inject(Router).navigate(['/music']);
    return false;
  }
  return true;
};
