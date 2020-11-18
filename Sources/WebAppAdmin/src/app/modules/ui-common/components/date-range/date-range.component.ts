import {ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Period} from '../../../core/models/period';
import {FormControl} from '@angular/forms';
import {Utils} from '../../../core/utils/utils';

@Component({
  selector: 'app-date-range',
  templateUrl: './date-range.component.html',
  styleUrls: ['./date-range.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DateRangeComponent implements OnInit {

  @Input() set startPeriod(value: Period) {
    this.startDateControl.setValue(value.fromDate, {emitEvent: false});
    this.endDateControl.setValue(value.toDate, {emitEvent: false});
  }
  @Input() readonly = true;
  startDateControl: FormControl = new FormControl();
  endDateControl: FormControl = new FormControl();
  @Output() selectedPeriodChanged = new EventEmitter<Period>();
  @Input() disabled = false;

  constructor() {
  }

  ngOnInit() {
  }

  onPeriodChanged() {
    const fromDate = this.startDateControl.value as Date;
    const toDate = this.endDateControl.value as Date;
    if (!Utils.isNullOrUndefined(fromDate) && !Utils.isNullOrUndefined(toDate)) {
      this.selectedPeriodChanged.emit({fromDate, toDate});
    }
  }
}
