import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { Task } from '../../core/models/task.model';
import { TaskStatusTypes } from '../enums/task-status.enum';
import { TaskPriorityTypes } from '../enums/task-priority.enum';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.scss'],
  imports: [CommonModule, SharedModule],
  standalone: true,
})
export class TaskDetailComponent implements OnInit {
  protected task = signal<Task | null>(null);
  protected isLoading = signal<boolean>(true);
  protected error = signal<string | null>(null);
  protected showDeleteConfirm = signal<boolean>(false);

  protected statusTypes = TaskStatusTypes;
  protected priorityTypes = TaskPriorityTypes;

  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  public ngOnInit(): void {
    const taskId = this.route.snapshot.paramMap.get('id');
    if (taskId) {
      this.loadTask(taskId);
    } else {
      this.error.set('Task ID is missing');
      this.isLoading.set(false);
    }
  }

  protected loadTask(id: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.taskService.getTaskById(id).subscribe({
      next: (task) => {
        this.task.set(task);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load task');
        this.isLoading.set(false);
      },
    });
  }

  protected editTask(): void {
    if (this.task()) {
      this.router.navigate(['/tasks', this.task()!.id, 'edit']);
    }
  }

  protected confirmDelete(): void {
    this.showDeleteConfirm.set(true);
  }

  protected onDeleteConfirm(): void {
    if (this.task()) {
      this.deleteTask(this.task()!.id);
    }
  }

  protected onDeleteCancel(): void {
    this.showDeleteConfirm.set(false);
  }

  protected deleteTask(id: string): void {
    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to delete task');
        this.showDeleteConfirm.set(false);
      },
    });
  }

  protected changeStatus(statusId: number): void {
    if (!this.task() || this.task()!.status === statusId) return;

    this.taskService.changeTaskStatus(this.task()!.id, statusId).subscribe({
      next: (updatedTask) => {
        this.task.set(updatedTask);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to update task status');
      },
    });
  }
}
