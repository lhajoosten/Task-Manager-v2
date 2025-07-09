import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProfileComponent } from './profile/profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { SharedModule } from '../shared/shared.module';
import { authRoutes } from './auth.routes';

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(authRoutes),
    SharedModule,
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    ChangePasswordComponent,
  ],
})
export class AuthModule {}
