import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable, of} from 'rxjs';
import {AdminUserModel} from '../models/admin-user.model';
import {catchError, map} from 'rxjs/operators';
import {ApiService} from './api.service';
import {Router} from '@angular/router';
import {StorageService} from './storage.service';

@Injectable()
export class AuthenticationService {

  private currentAdminUserSubject: BehaviorSubject<AdminUserModel>;
  currentAdminUser: Observable<AdminUserModel>;

  constructor(private api: ApiService,
              private storage: StorageService) {
    this.currentAdminUserSubject = new BehaviorSubject<AdminUserModel>(JSON.parse(storage.getFromRepository('currentAdminUser')));
    this.currentAdminUser = this.currentAdminUserSubject.asObservable();
  }

  get currentAdminUserValue(): AdminUserModel {
    return this.currentAdminUserSubject.value;
  }

  login(adminUser: AdminUserModel): Observable<AdminUserModel> {
    const url = `admin/user/login`;
    return this.api.post(url, adminUser).pipe(
      map(user => {
        this.storage.saveInRepository('confirmUser', JSON.stringify(adminUser));
        return user;
      })
    );
  }

  confirmLogin(verificationCode: number): Observable<AdminUserModel> {
    const url = 'admin/user/confirm/login';
    const user = JSON.parse(this.storage.getFromRepository('confirmUser')) as AdminUserModel;
    user.verificationCode = verificationCode;
    return this.api.post(url, user).pipe(
      map(obj => {
        const adminUserModel = obj as AdminUserModel;
        this.storage.removeFromRepository('confirmUser');
        this.storage.saveInRepository('currentAdminUser', JSON.stringify(adminUserModel));
        this.currentAdminUserSubject.next(adminUserModel);
        return adminUserModel;
      })
    );
  }

  logout() {
    this.currentAdminUserSubject.next(null);
    this.storage.removeFromRepository('currentAdminUser');
  }

}
