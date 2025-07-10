import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const ErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        [401, 403].includes(error.status) &&
        authService.isLoggedIn?.()
      ) {
        authService.logout?.();
      }
      const errorMessage =
        error.error?.error || error.statusText || 'An error occurred';
      return throwError(() => new Error(errorMessage));
    }),
  );
};
