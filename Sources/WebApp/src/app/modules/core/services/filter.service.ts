import {EventEmitter, Injectable} from '@angular/core';
import {Period} from '../models/period';
import {MasterAccountModel} from '../models/master-account.model';
import {TradeAccountModel} from '../models/trade-account.model';

@Injectable()
export class FilterService {

  private filtersChangedEventEmmiter = new EventEmitter<FilterEvent>();

  private _selectedPeriod: Period;
  private _selectedMasterAccounts: MasterAccountModel[];
  private _selectedTradeAccounts: TradeAccountModel[];

  constructor() {
    this.setDefault();
  }

  get filtersChangedEvent() {
    return this.filtersChangedEventEmmiter.asObservable();
  }

  setSelectedPeriod(value: Period, emmitEvent: boolean = true) {
    this._selectedPeriod = value;
    if (emmitEvent) this.filtersChangedEventEmmiter.emit({isPeriodChanged: true});
  }

  get selectedPeriod() {
    return this._selectedPeriod;
  }

  setSelectedMasterAccounts(value: MasterAccountModel[], emmitEvent: boolean = true) {
    this._selectedMasterAccounts = value;
    this._selectedTradeAccounts = [];
    if (emmitEvent) this.filtersChangedEventEmmiter.emit({isMasterAccountChanged: true});
  }

  get selectedMasterAccounts() {
    return this._selectedMasterAccounts;
  }

  setSelectedTradeAccounts(value: TradeAccountModel[], emmitEvent: boolean = true) {
    this._selectedTradeAccounts = value;
    this._selectedMasterAccounts = [MasterAccountModel.getDefault()];
    if (emmitEvent) this.filtersChangedEventEmmiter.emit({isTradeAccountChanged: true});
  }

  get selectedTradeAccounts() {
    return this._selectedTradeAccounts;
  }

  resetFilter(emmitEvent: boolean = false) {
    this.setDefault();
    if (emmitEvent) this.filtersChangedEventEmmiter.emit();
  }

  emitEvent() {
    this.filtersChangedEventEmmiter.emit();
  }

  private setDefault() {
    this._selectedPeriod = Period.getDefault();
    this._selectedMasterAccounts = [MasterAccountModel.getDefault()];
    this._selectedTradeAccounts = [];
  }

}

export class FilterEvent {
  isMasterAccountChanged? = false;
  isTradeAccountChanged? = false;
  isPeriodChanged? = false;
}
