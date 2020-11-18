import {Pipe, PipeTransform} from '@angular/core';
import {DateConvector} from '../../core/utils/date-convector';

@Pipe({
  name: 'dateToString'
})
export class DateToStringPipe implements PipeTransform {

  transform(value: Date, ...args: any[]): string {
    return DateConvector.DateToString(value);
  }

}
