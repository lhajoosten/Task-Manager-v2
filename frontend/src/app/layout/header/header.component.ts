import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  imports: [RouterModule],
  standalone: true,
})
export class HeaderComponent {
  protected isMenuOpen = false;

  protected authService = inject(AuthService);
  private router = inject(Router);

  protected toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  protected closeMenu(): void {
    this.isMenuOpen = false;
  }

  protected logout(): void {
    this.authService.logout();
    this.closeMenu();
  }

  protected navigateTo(path: string): void {
    this.router.navigate([path]);
    this.closeMenu();
  }
}
