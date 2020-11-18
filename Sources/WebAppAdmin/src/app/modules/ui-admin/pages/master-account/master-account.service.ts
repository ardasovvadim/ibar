import { Injectable } from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {Observable, of} from 'rxjs';
import {UserModel} from '../user/user.model';
import {catchError, map} from 'rxjs/operators';
import {MasterAccountGridVm} from './master-account-grid.vm';

@Injectable()
export class MasterAccountService {

  private masterAccounts: MasterAccountGridVm[] = null;

  constructor(private api: ApiService) { }

  getAllMasterAccounts(refresh: boolean = false): Observable<MasterAccountGridVm[]> {
    if (refresh || this.masterAccounts == null) {
      const obs = this.api.get('admin/accounts/master/all').pipe(
        catchError(err => of([]))
      );
      obs.subscribe(data => this.masterAccounts = data);
      return obs;
    }
    return of(this.masterAccounts);
  }

  addNewMasterAccount(masterAccount: MasterAccountGridVm): Observable<number> {
    return this.api.post('admin/accounts/master/add', masterAccount);
  }

  deleteMasterAccount(masterAccount: MasterAccountGridVm): Observable<number> {
    return this.api.delete('admin/accounts/master/delete/' + masterAccount.id);
  }

  editMasterAccount(masterAccount: MasterAccountGridVm): Observable<number> {
    return this.api.put('admin/accounts/master/update', masterAccount);
  }
}
