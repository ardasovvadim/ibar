import { Injectable } from '@angular/core';
import {ApiService} from './api.service';
import {MasterAccountModel} from '../models/master-account.model';
import {catchError, map} from 'rxjs/operators';
import {Observable, of} from 'rxjs';

@Injectable()
export class MasterAccountService {

  constructor(private api: ApiService) {}

  getMasterAccounts(): Observable<MasterAccountModel[]> {
    return this.api.get(`masteraccount/getall/`).pipe(catchError(err => of([])));
  }

}
