export interface Task {
  id: string;
  title: string;
  description: string;
  dueDate?: string;
  status: number;
  priority: number;
  assignedTo: string;
  createdAt: string;
  updatedAt?: string;
  isOverdue: boolean;
}
