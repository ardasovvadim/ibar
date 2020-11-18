import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customCurrency'
})
export class CustomCurrencyPipe implements PipeTransform {

  transform(value: string, ...args: any[]): string {
    if (typeof value !== 'undefined' && value != null) {
      return value.replace('$', '$ ').replace(/,/g, ' ').replace(/\./g, ',');
    } else {
      return value;
    }
  }

}
