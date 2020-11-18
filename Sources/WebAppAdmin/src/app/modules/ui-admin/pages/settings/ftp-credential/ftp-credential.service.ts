import {Injectable} from '@angular/core';
import {ApiService} from '../../../../core/services/api.service';
import {BehaviorSubject, Observable, of} from 'rxjs';
import {FtpCredentialModel} from './ftp-credential.model';
import {MasterAccountGridVm} from '../../master-account/master-account-grid.vm';
import {catchError} from 'rxjs/operators';
import {IdNameModel} from '../../../../core/models/id-name.model';

@Injectable({
  providedIn: 'root'
})
export class FtpCredentialService {

  private readonly controllerPath = 'ftp';
  private masterAccounts = new BehaviorSubject<MasterAccountGridVm[]>(null);

  constructor(private api: ApiService) {
    this.api.get('masteraccount/GetAll')
      .pipe(catchError(err => of([])))
      .subscribe(data => this.masterAccounts.next(data));
  }

  public getMasterAccounts(): Observable<MasterAccountGridVm[]> {
    return this.masterAccounts.asObservable();
  }

  public getAllFtpCredentials(): Observable<FtpCredentialModel[]> {
    return this.api.get('ftp/GetAllFtpCredentials');
  }

  addFtpCredential(ftpCred: FtpCredentialModel): Observable<FtpCredentialModel> {
    return this.api.post('ftp/AddNewFtpCredential', ftpCred);
  }

  deleteFtpCredential(ftpCred: FtpCredentialModel): Observable<void> {
    return this.api.delete(`ftp/DeleteFtpCredential/${ftpCred.id}`);
  }

  updateFtpCredential(ftpCred: FtpCredentialModel): Observable<FtpCredentialModel> {
    return this.api.put('ftp/UpdateFtpCredential', ftpCred);
  }

  getIdNames(): Observable<IdNameModel[]> {
    return this.api.get(`${this.controllerPath}/id-name`);
  }
}
