import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {Observable, of, pipe} from 'rxjs';
import {UserModel} from '../models/user.model';
import {catchError, map} from 'rxjs/operators';
import {environment} from '../../../../environments/environment';
import {StorageService} from './storage.service';

@Injectable()
export class RegistrationService {

  constructor(private api: ApiService, private storage: StorageService) {
  }

  sendRegistrationCode(): Observable<void> {
    const linkKey = this.storage.getFromRepository('linkKey');
    return this.api.get(`registration/SendRegistrationCode/${linkKey}`);
  }

  confirmRegistration(phoneCode: number): Observable<UserModel> {
    const linkKey = this.storage.getFromRepository('linkKey');
    return this.api.post('registration/ConfirmRegistration', {Item1: linkKey, Item2: phoneCode}).pipe(
      map(user => {
        this.storage.saveInRepository('registrationUser', JSON.stringify(user));
        return user;
      })
    );
  }

  finishRegistration(user: UserModel): Observable<string> {
    const linkKey = this.storage.getFromRepository('linkKey');
    return this.api.post('registration/FinishRegistration', {Item1: linkKey, Item2: user}).pipe(
      map(data => data === 'admin' ? environment.urlWebAppAdmin : '/')
    );
  }

  isWaitingConfirmation(linkKey: string): Observable<boolean> {
    return this.api.get(`registration/IsWaitingConfirmation/${linkKey}`);
  }

  Clear(): void {
    this.storage.removeFromRepository('linkKey');
    this.storage.removeFromRepository('registrationUser');
  }
}
