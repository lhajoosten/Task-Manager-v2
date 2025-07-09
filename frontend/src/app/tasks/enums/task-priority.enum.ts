export enum TaskPriorityTypes {
  LOW = 1,
  MEDIUM = 2,
  HIGH = 3,
  URGENT = 4,
}

export const TaskPriorityLabels: Record<TaskPriorityTypes, string> = {
  [TaskPriorityTypes.LOW]: 'Low',
  [TaskPriorityTypes.MEDIUM]: 'Medium',
  [TaskPriorityTypes.HIGH]: 'High',
  [TaskPriorityTypes.URGENT]: 'Urgent',
};
