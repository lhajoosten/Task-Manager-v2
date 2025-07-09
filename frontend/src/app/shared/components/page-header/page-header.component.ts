import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-page-header',
  templateUrl: './page-header.component.html',
  styleUrls: ['./page-header.component.scss'],
  imports: [CommonModule, FormsModule, RouterModule],
  standalone: true,
})
export class PageHeaderComponent {
  @Input() public title = '';
  @Input() public subtitle = '';
  @Input() public buttonText = '';
  @Input() public buttonLink = '';
  @Input() public buttonIcon = '';
}
