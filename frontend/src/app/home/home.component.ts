import { Component, inject, effect } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [RouterModule],
  standalone: true,
})
export class HomeComponent {
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    effect(() => {
      if (this.authService.isInitialized() && this.authService.isLoggedIn()) {
        this.router.navigate(['/tasks']);
      }
    });
  }
}
