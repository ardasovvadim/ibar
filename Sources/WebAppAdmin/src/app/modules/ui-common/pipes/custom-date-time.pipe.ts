import { Pipe, PipeTransform } from '@angular/core';
import {DateConvector} from '../../core/utils/date-convector';

@Pipe({
  name: 'customDateTime'
})
export class CustomDateTimePipe implements PipeTransform {

  transform(value: Date, ...args: unknown[]): string {
    return DateConvector.dateTimeToString(value);
  }

}
