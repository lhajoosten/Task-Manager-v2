export interface NavItem {
  label: string;
  route: string;
  icon?: string;
  children?: NavItem[];
}

export interface PagedRequest {
  page: number;
  pageSize: number;
}

export interface AlertMessage {
  type: 'success' | 'info' | 'warning' | 'danger';
  message: string;
  dismissible?: boolean;
  timeout?: number;
}
