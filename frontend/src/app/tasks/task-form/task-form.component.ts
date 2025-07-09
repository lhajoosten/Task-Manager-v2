import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { Task } from '../../core/models/task.model';
import { TaskPriorityTypes } from '../enums/task-priority.enum';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss'],
  imports: [CommonModule, SharedModule],
  standalone: true,
})
export class TaskFormComponent implements OnInit {
  protected taskForm: FormGroup;
  protected isEditMode = signal<boolean>(false);
  protected taskId = signal<string | null>(null);
  protected isLoading = signal<boolean>(false);
  protected isSaving = signal<boolean>(false);
  protected error = signal<string | null>(null);

  protected priorityOptions = Object.entries(TaskPriorityTypes)
    .filter(([key]) => !isNaN(Number(key)))
    .map(([key, value]) => ({ id: Number(key), name: value }));

  private readonly formBuilder = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  constructor() {
    this.taskForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(1000)],
      dueDate: [null],
      priority: [TaskPriorityTypes.MEDIUM],
    });
  }

  public ngOnInit(): void {
    this.taskId.set(this.route.snapshot.paramMap.get('id'));
    this.isEditMode.set(!!this.taskId());

    if (this.isEditMode()) {
      this.loadTask(this.taskId()!);
    }
  }

  protected loadTask(id: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.taskService.getTaskById(id).subscribe({
      next: (task) => {
        this.patchForm(task);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load task');
        this.isLoading.set(false);
      },
    });
  }

  protected patchForm(task: Task): void {
    // Format date for the date input
    let dueDate = null;
    if (task.dueDate) {
      dueDate = new Date(task.dueDate).toISOString().split('T')[0];
    }

    this.taskForm.patchValue({
      title: task.title,
      description: task.description,
      dueDate: dueDate,
      priority: task.priority,
    });
  }

  protected onSubmit(): void {
    if (this.taskForm.invalid || this.isSaving()) {
      return;
    }

    this.isSaving.set(true);
    this.error.set(null);

    const taskData = this.prepareTaskData();

    if (this.isEditMode()) {
      this.updateTask(this.taskId()!, taskData);
    } else {
      this.createTask(taskData);
    }
  }

  protected prepareTaskData(): Partial<Task> {
    const formValue = this.taskForm.value;

    return {
      title: formValue.title,
      description: formValue.description || '',
      dueDate: formValue.dueDate
        ? new Date(formValue.dueDate).toISOString()
        : undefined,
      priority: formValue.priority,
    };
  }

  protected createTask(taskData: Partial<Task>): void {
    this.taskService.createTask(taskData).subscribe({
      next: (task) => {
        this.router.navigate(['/tasks', task.id]);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to create task');
        this.isSaving.set(false);
      },
    });
  }

  protected updateTask(id: string, taskData: Partial<Task>): void {
    this.taskService.updateTask(id, taskData).subscribe({
      next: (task) => {
        this.router.navigate(['/tasks', task.id]);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to update task');
        this.isSaving.set(false);
      },
    });
  }

  protected cancel(): void {
    if (this.isEditMode()) {
      this.router.navigate(['/tasks', this.taskId()]);
    } else {
      this.router.navigate(['/tasks']);
    }
  }
}
