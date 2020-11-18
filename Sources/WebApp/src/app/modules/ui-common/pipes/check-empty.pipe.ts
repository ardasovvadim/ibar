import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'checkEmpty'
})
export class CheckEmptyPipe implements PipeTransform {

  transform(value: string | number, ...args: any[]): any {
    return typeof value === 'undefined' || value == null ? '' : value;
  }

}
