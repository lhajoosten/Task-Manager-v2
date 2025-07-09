import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Task } from '../../core/models/task.model';
import { TaskService } from '../../core/services/task.service';
import { TaskStatusTypes } from '../enums/task-status.enum';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-task-item',
  templateUrl: './task-item.component.html',
  styleUrls: ['./task-item.component.scss'],
  imports: [CommonModule, SharedModule],
  standalone: true,
})
export class TaskItemComponent {
  @Input() task!: Task;
  @Output() delete = new EventEmitter<Task>();

  protected statusTypes = TaskStatusTypes;

  private taskService = inject(TaskService);
  private router = inject(Router);

  protected markComplete(): void {
    this.changeStatus(TaskStatusTypes.COMPLETED);
  }

  protected markInProgress(): void {
    this.changeStatus(TaskStatusTypes.IN_PROGRESS);
  }

  protected markTodo(): void {
    this.changeStatus(TaskStatusTypes.TODO);
  }

  protected markCancelled(): void {
    this.changeStatus(TaskStatusTypes.CANCELLED);
  }

  protected onDelete(): void {
    this.delete.emit(this.task);
  }

  protected viewDetails(): void {
    this.router.navigate(['/tasks', this.task.id]);
  }

  protected editTask(): void {
    this.router.navigate(['/tasks', this.task.id, 'edit']);
  }

  private changeStatus(statusId: number): void {
    if (this.task.status === statusId) return;

    this.taskService.changeTaskStatus(this.task.id, statusId).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
      },
      error: (error) => {
        console.error('Failed to update task status:', error);
      },
    });
  }
}
