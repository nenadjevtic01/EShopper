import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { currentRole } from '../Utils/session.util';
import { ToastService } from '../Services/toast.service';

export const customerGuard: CanActivateFn = () => {
  const router = inject(Router);
  const jwtHelper = inject(JwtHelperService);
  const toastService = inject(ToastService);

  if (currentRole(jwtHelper) === 'admin') {
    toastService.error('This page is available to customers only.');
    return router.createUrlTree(['/home']);
  }

  return true;
};
