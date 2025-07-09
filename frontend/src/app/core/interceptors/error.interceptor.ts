import { inject, Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private readonly authService = inject(AuthService);

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler,
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (
          [401, 403].includes(error.status) &&
          this.authService.isLoggedIn()
        ) {
          this.authService.logout();
        }
        const errorMessage =
          error.error?.error || error.statusText || 'An error occurred';
        return throwError(() => new Error(errorMessage));
      }),
    );
  }
}
