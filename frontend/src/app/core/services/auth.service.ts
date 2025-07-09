import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { User } from '../models/user.model';
import { AuthResponse } from '../models/auth-response.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly tokenKey = 'auth_token';

  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Use signals for reactive state management
  currentUser = signal<User | null>(null);
  isAuthenticated = signal<boolean>(false);

  constructor() {
    this.checkAuthStatus();
  }

  register(
    email: string,
    password: string,
    confirmPassword: string,
    firstName: string,
    lastName: string,
  ): Observable<AuthResponse> {
    return this.http
      .post<{
        isSuccess: boolean;
        value: AuthResponse;
        error?: string;
      }>(`${this.apiUrl}/register`, {
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
      })
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Registration failed');
          }
          this.setSession(response.value);
          return response.value;
        }),
      );
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<{
        isSuccess: boolean;
        value: AuthResponse;
        error?: string;
      }>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Login failed');
          }
          this.setSession(response.value);
          return response.value;
        }),
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated();
  }

  getCurrentUser(): Observable<User> {
    if (this.currentUser()) {
      return of(this.currentUser()!);
    }

    return this.http
      .get<{
        isSuccess: boolean;
        value: User;
        error?: string;
      }>(`${this.apiUrl}/me`)
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to get user info');
          }
          this.currentUser.set(response.value);
          return response.value;
        }),
        catchError((error) => {
          this.logout();
          return throwError(() => error);
        }),
      );
  }

  changePassword(
    currentPassword: string,
    newPassword: string,
    confirmNewPassword: string,
  ): Observable<boolean> {
    return this.http
      .post<{
        isSuccess: boolean;
        error?: string;
      }>(`${this.apiUrl}/change-password`, {
        currentPassword,
        newPassword,
        confirmNewPassword,
      })
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to change password');
          }
          return true;
        }),
      );
  }

  private setSession(authResult: AuthResponse): void {
    localStorage.setItem(this.tokenKey, authResult.accessToken);
    this.currentUser.set(authResult.user);
    this.isAuthenticated.set(true);
  }

  private checkAuthStatus(): void {
    const token = this.getToken();
    if (token) {
      this.isAuthenticated.set(true);
      this.getCurrentUser().subscribe({
        error: () => this.logout(),
      });
    }
  }
}
