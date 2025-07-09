import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

import { TaskListComponent } from './task-list/task-list.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import { TaskFormComponent } from './task-form/task-form.component';
import { TaskItemComponent } from './task-item/task-item.component';
import { SharedModule } from '../shared/shared.module';
import { tasksRoutes } from './tasks.routes';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(tasksRoutes),
    SharedModule,
    TaskListComponent,
    TaskDetailComponent,
    TaskFormComponent,
    TaskItemComponent,
  ],
})
export class TasksModule {}
