import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {IdNameModel} from '../models/id-name.model';
import {ApiService} from './api.service';

@Injectable({
  providedIn: 'root'
})
export class MasterAccountService {

  private readonly controllerPath = 'masteraccount';

  constructor(private api: ApiService) { }

  getMasterAccountIdNamesModel(): Observable<IdNameModel[]> {
    return this.api.get(`${this.controllerPath}/id-name`);
  }
}
