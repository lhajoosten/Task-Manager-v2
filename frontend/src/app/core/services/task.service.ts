import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Task } from '../models/task.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private readonly apiUrl = `${environment.apiUrl}/tasks`;

  private readonly http = inject(HttpClient);

  public getTasks(
    page = 1,
    pageSize = 10,
    status?: number,
    priority?: number,
    search?: string,
    onlyOverdue = false,
  ): Observable<PagedResult<Task>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('onlyOverdue', onlyOverdue.toString());

    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    if (priority !== undefined) {
      params = params.set('priority', priority.toString());
    }
    if (search) {
      params = params.set('search', search);
    }

    return this.http
      .get<{
        isSuccess: boolean;
        value: PagedResult<Task>;
        error?: string;
      }>(this.apiUrl, { params })
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to fetch tasks');
          }
          return response.value;
        }),
      );
  }

  public getTaskById(id: string): Observable<Task> {
    return this.http
      .get<{
        isSuccess: boolean;
        value: Task;
        error?: string;
      }>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to fetch task');
          }
          return response.value;
        }),
      );
  }

  public createTask(task: Partial<Task>): Observable<Task> {
    return this.http
      .post<{
        isSuccess: boolean;
        value: Task;
        error?: string;
      }>(this.apiUrl, task)
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to create task');
          }
          return response.value;
        }),
      );
  }

  public updateTask(id: string, task: Partial<Task>): Observable<Task> {
    return this.http
      .put<{
        isSuccess: boolean;
        value: Task;
        error?: string;
      }>(`${this.apiUrl}/${id}`, task)
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to update task');
          }
          return response.value;
        }),
      );
  }

  public changeTaskStatus(id: string, status: number): Observable<Task> {
    return this.http
      .patch<{
        isSuccess: boolean;
        value: Task;
        error?: string;
      }>(`${this.apiUrl}/${id}/status`, { status })
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to change task status');
          }
          return response.value;
        }),
      );
  }

  public deleteTask(id: string): Observable<boolean> {
    return this.http
      .delete<{ isSuccess: boolean; error?: string }>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (!response.isSuccess) {
            throw new Error(response.error || 'Failed to delete task');
          }
          return true;
        }),
      );
  }
}
