import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

export type AlertType = 'success' | 'info' | 'warning' | 'danger';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss'],
  imports: [CommonModule],
  standalone: true,
})
export class AlertComponent {
  @Input() public type: AlertType = 'info';
  @Input() public message = '';
  @Input() public dismissible = true;

  @Output() public closing = new EventEmitter<void>();

  public onClose(): void {
    this.closing.emit();
  }
}
