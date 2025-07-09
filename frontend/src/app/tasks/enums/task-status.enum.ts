export enum TaskStatusTypes {
  PENDING = 1,
  IN_PROGRESS = 2,
  COMPLETED = 3,
  CANCELLED = 4,
  TODO = 5,
}

export const TaskStatusLabels: Record<TaskStatusTypes, string> = {
  [TaskStatusTypes.PENDING]: 'Pending',
  [TaskStatusTypes.IN_PROGRESS]: 'In Progress',
  [TaskStatusTypes.COMPLETED]: 'Completed',
  [TaskStatusTypes.CANCELLED]: 'Cancelled',
  [TaskStatusTypes.TODO]: 'To Do',
};
