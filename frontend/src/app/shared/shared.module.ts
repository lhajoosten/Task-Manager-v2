import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

import { AlertComponent } from './components/alert/alert.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { TaskStatusPipe } from './pipes/task-status.pipe';
import { TaskPriorityPipe } from './pipes/task-priority.pipe';
import { DateTimeFormatPipe } from './pipes/date-time-format.pipe';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,

    AlertComponent,
    LoadingSpinnerComponent,
    PageHeaderComponent,
    ConfirmDialogComponent,
    TaskStatusPipe,
    TaskPriorityPipe,
    DateTimeFormatPipe,
  ],
  exports: [
    AlertComponent,
    LoadingSpinnerComponent,
    PageHeaderComponent,
    ConfirmDialogComponent,
    TaskStatusPipe,
    TaskPriorityPipe,
    DateTimeFormatPipe,
    RouterModule,
    ReactiveFormsModule,
  ],
})
export class SharedModule {}
