import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
  name: 'checkEmpty'
})
export class CheckEmptyPipe implements PipeTransform {

  transform(value: any, ...args: any[]): any {
    return typeof value === 'undefined' || value == null ? '' : value;
  }

}
