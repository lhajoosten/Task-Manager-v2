import { Routes } from '@angular/router';
import { TaskListComponent } from './task-list/task-list.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import { TaskFormComponent } from './task-form/task-form.component';
import { authGuard } from '../core/guards/auth.guard';

export const tasksRoutes: Routes = [
  {
    path: '',
    component: TaskListComponent,
    canActivate: [authGuard],
  },
  {
    path: 'new',
    component: TaskFormComponent,
    canActivate: [authGuard],
  },
  {
    path: ':id',
    component: TaskDetailComponent,
    canActivate: [authGuard],
  },
  {
    path: ':id/edit',
    component: TaskFormComponent,
    canActivate: [authGuard],
  },
];
