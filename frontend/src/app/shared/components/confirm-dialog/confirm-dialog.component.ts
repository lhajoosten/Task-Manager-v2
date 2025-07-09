import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss'],
  imports: [CommonModule, RouterModule],
  standalone: true,
})
export class ConfirmDialogComponent {
  @Input() public isOpen = false;
  @Input() public title = 'Confirm Action';
  @Input() public message = 'Are you sure you want to proceed?';
  @Input() public confirmButtonText = 'Confirm';
  @Input() public cancelButtonText = 'Cancel';
  @Input() public confirmButtonClass = 'btn-danger';

  @Output() public confirming = new EventEmitter<void>();
  @Output() public canceling = new EventEmitter<void>();

  protected onConfirm(): void {
    this.confirming.emit();
  }

  protected onCancel(): void {
    this.canceling.emit();
  }
}
