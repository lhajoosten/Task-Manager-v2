import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'dateTimeFormat',
})
export class DateTimeFormatPipe implements PipeTransform {
  transform(value: string | Date | null, format = 'medium'): string {
    if (!value) {
      return '';
    }

    const datePipe = new DatePipe('nl-NL');
    return datePipe.transform(value, format) || '';
  }
}
