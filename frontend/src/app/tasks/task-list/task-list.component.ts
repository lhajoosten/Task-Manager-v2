import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { TaskService } from '../../core/services/task.service';
import { Task } from '../../core/models/task.model';
import { PagedResult } from '../../core/models/paged-result.model';
import { TaskStatusTypes } from '../enums/task-status.enum';
import { TaskPriorityTypes } from '../enums/task-priority.enum';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { TaskItemComponent } from '../task-item/task-item.component';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss'],
  imports: [
    CommonModule,
    SharedModule,
    ConfirmDialogComponent,
    TaskItemComponent,
  ],
  standalone: true,
})
export class TaskListComponent implements OnInit {
  protected tasks = signal<Task[]>([]);
  protected isLoading = signal<boolean>(false);
  protected totalTasks = signal<number>(0);
  protected currentPage = signal<number>(1);
  protected pageSize = signal<number>(10);
  protected totalPages = signal<number>(0);
  protected error = signal<string | null>(null);

  // For filtering and search
  protected filterForm: FormGroup;
  protected statusOptions = Object.entries(TaskStatusTypes)
    .filter(([key]) => !isNaN(Number(key)))
    .map(([key, value]) => ({ id: Number(key), name: value }));

  protected priorityOptions = Object.entries(TaskPriorityTypes)
    .filter(([key]) => !isNaN(Number(key)))
    .map(([key, value]) => ({ id: Number(key), name: value }));

  // For deletion confirmation
  protected showDeleteConfirm = signal<boolean>(false);
  protected taskToDelete = signal<Task | null>(null);

  private readonly taskService = inject(TaskService);
  private readonly formBuilder = inject(FormBuilder);

  constructor() {
    this.filterForm = this.formBuilder.group({
      search: [''],
      status: [null],
      priority: [null],
      onlyOverdue: [false],
    });
  }

  ngOnInit(): void {
    this.loadTasks();

    // Apply filters when form values change
    this.filterForm
      .get('search')
      ?.valueChanges.pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        this.resetPagination();
        this.loadTasks();
      });

    this.filterForm.get('status')?.valueChanges.subscribe(() => {
      this.resetPagination();
      this.loadTasks();
    });

    this.filterForm.get('priority')?.valueChanges.subscribe(() => {
      this.resetPagination();
      this.loadTasks();
    });

    this.filterForm.get('onlyOverdue')?.valueChanges.subscribe(() => {
      this.resetPagination();
      this.loadTasks();
    });
  }

  loadTasks(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const { search, status, priority, onlyOverdue } = this.filterForm.value;

    this.taskService
      .getTasks(
        this.currentPage(),
        this.pageSize(),
        status,
        priority,
        search,
        onlyOverdue,
      )
      .subscribe({
        next: (result: PagedResult<Task>) => {
          this.tasks.set(result.items);
          this.totalTasks.set(result.totalCount);
          this.totalPages.set(result.totalPages);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.error.set(err.message || 'Failed to load tasks');
          this.isLoading.set(false);
        },
      });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadTasks();
  }

  resetPagination(): void {
    this.currentPage.set(1);
  }

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: null,
      priority: null,
      onlyOverdue: false,
    });
    this.resetPagination();
    this.loadTasks();
  }

  confirmDelete(task: Task): void {
    this.taskToDelete.set(task);
    this.showDeleteConfirm.set(true);
  }

  onDeleteConfirm(): void {
    if (this.taskToDelete()) {
      this.deleteTask(this.taskToDelete()!.id);
      this.showDeleteConfirm.set(false);
      this.taskToDelete.set(null);
    }
  }

  onDeleteCancel(): void {
    this.showDeleteConfirm.set(false);
    this.taskToDelete.set(null);
  }

  deleteTask(id: string): void {
    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.tasks.update((tasks) => tasks.filter((task) => task.id !== id));
        this.totalTasks.update((count) => count - 1);

        // If we've deleted the last item on the page, go to previous page
        if (this.tasks().length === 0 && this.currentPage() > 1) {
          this.currentPage.update((page) => page - 1);
          this.loadTasks();
        }
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to delete task');
      },
    });
  }
}
