import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { currentRole } from '../Utils/session.util';

export const adminGuard: CanActivateFn = () => {
  const router = inject(Router);
  const jwtHelper = inject(JwtHelperService);

  if (currentRole(jwtHelper) !== 'admin') {
    return router.createUrlTree(['/home']);
  }

  return true;
};
