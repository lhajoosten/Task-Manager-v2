import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  imports: [CommonModule, FormsModule, RouterModule, SharedModule],
  standalone: true,
})
export class ChangePasswordComponent {
  protected passwordForm: FormGroup;
  protected isLoading = signal<boolean>(false);
  protected error = signal<string | null>(null);
  protected success = signal<string | null>(null);

  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    this.passwordForm = this.formBuilder.group(
      {
        currentPassword: ['', Validators.required],
        newPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/),
          ],
        ],
        confirmNewPassword: ['', Validators.required],
      },
      {
        validator: this.mustMatch('newPassword', 'confirmNewPassword'),
      },
    );
  }

  // Custom validator to check if password and confirm password match
  mustMatch(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (matchingControl.errors && !matchingControl.errors['mustMatch']) {
        return;
      }

      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      } else {
        matchingControl.setErrors(null);
      }
    };
  }

  onSubmit(): void {
    if (this.passwordForm.invalid || this.isLoading()) {
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);
    this.success.set(null);

    const { currentPassword, newPassword, confirmNewPassword } =
      this.passwordForm.value;

    this.authService
      .changePassword(currentPassword, newPassword, confirmNewPassword)
      .subscribe({
        next: () => {
          this.success.set('Password changed successfully');
          this.isLoading.set(false);
          this.passwordForm.reset();
        },
        error: (err) => {
          this.error.set(err.message || 'Failed to change password');
          this.isLoading.set(false);
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/profile']);
  }
}
