import { Pipe, PipeTransform } from '@angular/core';
import {Utils} from '../../core/utils/utils';
import {DateConvector} from '../../core/utils/date-convector';

@Pipe({
  name: 'customDate'
})
export class CustomDatePipe implements PipeTransform {

  transform(value: Date, ...args: unknown[]): string {
    return !Utils.isNullOrUndefined(value) ? DateConvector.toString(value) : '';
  }

}
