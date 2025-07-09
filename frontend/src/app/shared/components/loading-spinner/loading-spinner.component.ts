import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss'],
  imports: [CommonModule],
  standalone: true,
})
export class LoadingSpinnerComponent {
  @Input() public size: 'sm' | 'md' | 'lg' = 'md';
  @Input() public fullPage = false;
  @Input() public message = 'Loading...';
}
