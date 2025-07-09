import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CoreModule } from './core/core.module';
import { LayoutModule } from './layout/layout.module';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CoreModule, LayoutModule],
  template: '<router-outlet></router-outlet>',
})
export class App {
  protected title = 'Task Manager';
}
