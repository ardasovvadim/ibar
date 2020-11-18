import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {TradeAccountModel} from '../models/trade-account.model';
import {Observable} from 'rxjs';

@Injectable()
export class TradeAccountService {

  private tradeAccounts: TradeAccountModel[] = [];
  // take first 5 element -> on API too
  private readonly amountCache = 5;

  constructor(private api: ApiService) {
  }

  getTradeAccountBySearchName(searchName: string): Observable<TradeAccountModel> {
    return this.api.get(`tradeaccount/GetBySearchName/${searchName}`);
  }

}
