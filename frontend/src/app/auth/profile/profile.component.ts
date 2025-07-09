import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
  imports: [CommonModule, FormsModule, RouterModule, SharedModule],
  standalone: true,
})
export class ProfileComponent implements OnInit {
  protected profileForm: FormGroup;
  protected isLoading = signal<boolean>(true);
  protected isSaving = signal<boolean>(false);
  protected error = signal<string | null>(null);
  protected success = signal<string | null>(null);
  protected user = signal<User | null>(null);

  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    this.profileForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: [{ value: '', disabled: true }],
    });
  }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        this.user.set(user);
        this.profileForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
        });
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load user profile');
        this.isLoading.set(false);
      },
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid || this.isSaving()) {
      return;
    }

    this.isSaving.set(true);
    this.error.set(null);
    this.success.set(null);

    // For now, simulate a successful update until the API is ready
    setTimeout(() => {
      this.success.set('Profile updated successfully');
      this.isSaving.set(false);

      // Update the user data in the service
      if (this.user()) {
        const updatedUser = {
          ...this.user()!,
          firstName: this.profileForm.value.firstName,
          lastName: this.profileForm.value.lastName,
          fullName: `${this.profileForm.value.firstName} ${this.profileForm.value.lastName}`,
        };
        this.authService.currentUser.set(updatedUser);
      }
    }, 1000);

    // implement the real update when API is ready
    /*
    this.authService.updateProfile(this.profileForm.value).subscribe({
      next: (updatedUser) => {
        this.user.set(updatedUser);
        this.success.set('Profile updated successfully');
        this.isSaving.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to update profile');
        this.isSaving.set(false);
      }
    });
    */
  }

  navigateToChangePassword(): void {
    this.router.navigate(['/change-password']);
  }
}
