import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {Period} from '../../../core/models/period';
import * as moment from 'moment';
import {SatDatepicker, SatDatepickerRangeValue} from 'saturn-datepicker';

@Component({
  selector: 'app-period-picker',
  templateUrl: './period-picker.component.html',
  styleUrls: ['./period-picker.component.sass']
})
export class PeriodPickerComponent implements OnInit {

  constructor() {
  }

  // for except some periods from list
  @Input() exceptPeriods?: string[];
  periods: Period[];
  // ngIf rangepicker
  @Input() isInput = true;
  // ngIf list periods
  @Input() isPeriods = true;
  @Input() rangeMode = true;

  // ngIf list periods
  @Input() isRangeSelect = true;
  @Output() selectedPeriodChange = new EventEmitter<Period>();
  selectedPeriod?: Period;
  @Input() isAdaptive = false;

  @ViewChild('picker', {static: false}) picker: SatDatepicker<Date>;
  selectedDates: SatDatepickerRangeValue<Date> = {
    begin: moment().toDate(),
    end: moment().add(1, 'day').toDate()
  };

  ngOnInit() {
    this.periods = Period.periods;
    if (this.exceptPeriods) {
      this.periods = this.periods.filter(period => !this.exceptPeriods.includes(period.name));
    }
    this.selectedPeriod = this.periods.find(period => period.name === 'MTD');
    this.periodToDates();
  }

  myFilter = (d: Date | null): boolean => {
    const day = (d || new Date()).getDay();
    // Prevent Saturday and Sunday from being selected.
    return day !== 0 && day !== 6;
  }

  private periodToDates(): void {
    this.selectedDates.begin = this.selectedPeriod.fromDate;
    this.selectedDates.end = this.selectedPeriod.toDate;
  }

  private datesToPeriod(): void {
    this.selectedPeriod = new Period('Custom', this.picker.beginDate, this.picker.endDate);
  }

  onClickPeriod(period: Period): void {
    this.selectedPeriod = period;
    this.periodToDates();
    this.picker._datepickerInput.value = this.selectedDates;
    this.picker.beginDate = this.selectedDates.begin;
    this.picker.endDate = this.selectedDates.end;
    this.selectedPeriodChange.emit(this.selectedPeriod);
  }

  periodChange() {
    this.datesToPeriod();
    this.selectedPeriodChange.emit(this.selectedPeriod);
  }
}
