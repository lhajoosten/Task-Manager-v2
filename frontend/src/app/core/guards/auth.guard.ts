import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { computed } from '@angular/core';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const authCheckComplete = computed(() => authService.isInitialized());

  return new Promise(resolve => {
    if (authCheckComplete()) {
      if (authService.isLoggedIn()) {
        resolve(true);
      } else {
        router.navigate(['/auth/login']);
        resolve(false);
      }
    } else {
      const checkAuth = () => {
        if (authCheckComplete()) {
          if (authService.isLoggedIn()) {
            resolve(true);
          } else {
            router.navigate(['/auth/login']);
            resolve(false);
          }
        } else {
          setTimeout(checkAuth, 10);
        }
      };
      checkAuth();
    }
  });
};
