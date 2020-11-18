import { Pipe, PipeTransform } from '@angular/core';
import {DateConvector} from '../../core/utils/date-convector';

@Pipe({
  name: 'datetimeToString'
})
export class DatetimeToStringPipe implements PipeTransform {

  transform(value: Date, ...args: any[]): string {
    return DateConvector.DateTimeToString(value);
  }

}
