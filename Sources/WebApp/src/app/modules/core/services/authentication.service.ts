import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {UserModel} from '../models/user.model';
import {ApiService} from './api.service';
import {map} from 'rxjs/operators';
import {StorageService} from './storage.service';
import {ChangePasswordModel} from '../../ui-common/pages/change-password-dialog/change-password.model';

@Injectable()
export class AuthenticationService {

  private readonly controllerPath = '';
  private currentUserSubject: BehaviorSubject<UserModel>;
  currentUser: Observable<UserModel>;

  constructor(private api: ApiService, private storage: StorageService) {
    this.currentUserSubject = new BehaviorSubject<UserModel>(JSON.parse(storage.getFromRepository('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  get currentUserValue(): UserModel {
    return this.currentUserSubject.value;
  }

  login(user: UserModel): Observable<UserModel> {
    const url = `auth/login`;
    return this.api.post(url, user).pipe(
      map(u => {
        this.storage.saveInRepository('confirmUser', JSON.stringify(user));
        return u;
      })
    );
  }

  confirmLogin(verificationCode: number): Observable<UserModel> {
    const url = 'auth/confirmlogin';
    const user = JSON.parse(this.storage.getFromRepository('confirmUser')) as UserModel;
    user.verificationCode = verificationCode;
    return this.api.post(url, user).pipe(
      map(obj => {
        const userModel = obj as UserModel;
        this.storage.removeFromRepository('confirmUser');
        this.storage.saveInRepository('currentUser', JSON.stringify(userModel));
        this.currentUserSubject.next(userModel);
        return userModel;
      })
    );
  }

  logout() {
    this.currentUserSubject.next(null);
    this.storage.removeFromRepository('currentUser');
  }

  changePassword(passwordModel: ChangePasswordModel): Observable<void> {
    return this.api.put(`user/change-password`, passwordModel);
  }
}
