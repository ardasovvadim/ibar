import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {DateConvector} from '../../../core/utils/date-convector';
import {FormControl} from '@angular/forms';
import * as moment from 'moment';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.sass']
})
export class DatePickerComponent implements OnInit {

  dateControl = new FormControl();
  @Output() dateChanged = new EventEmitter<string>();
  // ---
  @Input() dateExcept: Date[] = null;
  @Input() availableDates: Date[] = null;
  // ---
  @Input() isNavButtons = true;
  prevDate = false;
  nextDate = false;

  // ---
  @Input() set setDate(date: Date) {
    if (typeof date !== 'undefined' && date != null) {
      this.dateControl.setValue(date);
      this.emit();
    } else {
      this.dateControl.setValue(null, {emitEvent: false});
      this.prevDate = false;
      this.nextDate = false;
    }
  }

  constructor() {
  }

  filter = (d: Date | null): boolean => {
    if (d == null) return false;

    let except = false;
    let available = false;
    if (this.dateExcept && this.dateExcept.length > 0) {
      except = !this.dateExcept.find(f => moment(f).isSame(moment(d), 'days'));
    }
    if (this.availableDates && this.availableDates.length > 0) {
      available = !!this.availableDates.find(f => moment(f).isSame(moment(d), 'days'));
    }

    return (this.dateExcept && this.availableDates && except && available) ||
      (this.availableDates && available) ||
      (this.dateExcept && except);
  }


  ngOnInit() {

  }

  emit() {
    this.prevDate = this.isPrevDate();
    this.nextDate = this.isNextDate();
    const strDate = DateConvector.toUrlDate(this.dateControl.value as Date);
    this.dateChanged.emit(strDate);
  }

  isPrevDate(): boolean {
    const curr = this.dateControl.value as Date;
    return this.filter(this.getPrevDate(curr));
  }

  isNextDate() {
    const curr = this.dateControl.value as Date;
    return this.filter(this.getNextDate(curr));
  }

  setPrevDate() {
    if (this.prevDate) {
      const prevDate = this.getPrevDate(this.dateControl.value);
      this.dateControl.setValue(prevDate);
      this.emit();
    }
  }

  setNextDate() {
    if (this.nextDate) {
      const next = this.getNextDate(this.dateControl.value);
      this.dateControl.setValue(next);
      this.emit();
    }
  }

  getPrevDate(date: Date): Date {
    const index = this.availableDates.findIndex(d => moment(d).isSame(moment(date), 'days'));

    if (index !== -1 && index !== 0) {
      return this.availableDates[index - 1];
    }

    return null;
  }

  getNextDate(date: Date): Date {
    const index = this.availableDates.findIndex(d => moment(d).isSame(moment(date), 'days'));

    if (index !== -1 && index !== this.availableDates.length - 1) {
      return this.availableDates[index + 1];
    }

    return null;
  }
}
