import {Component, Input, OnInit} from '@angular/core';
import {DashboardEnum} from '../../../ui-reporting/pages/dashboard/dashboard.enum';
import {DashboardService} from '../../../core/services/dashboard.service';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.sass']
})
export class CardComponent implements OnInit {
  @Input() chartType: DashboardEnum;
  @Input() title: string;
  @Input() iconClass: string;
  @Input() totalValue: any = null;
  @Input() format: 'string' | 'money' = 'money';
  @Input() isClickable = false;

  constructor() {
  }

  ngOnInit() {
  }

  resolveValue(): string {
    if (typeof this.totalValue === 'undefined' || this.totalValue == null) {
      this.totalValue = '0';
    }
    switch (this.format) {
      case 'money':
        const number = this.totalValue;
        return this.formatMoney(number);
      case 'string':
        return this.totalValue;
    }
  }

  formatMoney(number: string): string {
    const index = number.indexOf('.');
    const less = number.substring(index + 1);
    console.log(less);
    const more = number.substring(0, index);
    console.log(more);
    return '';
  }

}
