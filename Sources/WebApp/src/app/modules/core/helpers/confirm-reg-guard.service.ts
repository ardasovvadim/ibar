import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable, of, throwError} from 'rxjs';
import {RegistrationService} from '../services/registration.service';
import {StorageService} from '../services/storage.service';
import {AuthenticationService} from '../services/authentication.service';
import {catchError} from 'rxjs/operators';
import {ExceptionHeaderService} from '../services/exception-header.service';

@Injectable()
export class ConfirmRegGuard implements CanActivate {

  constructor(private registrationService: RegistrationService,
              private router: Router,
              private storage: StorageService,
              private authenticationService: AuthenticationService,
              private exceptionHeaderService: ExceptionHeaderService) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.authenticationService.currentUserValue !== null) {
      this.authenticationService.logout();
    }

    let linkKey = this.storage.getFromRepository('linkKey');

    if (linkKey == null) {
      linkKey = next.url.pop().path;

      if (!linkKey) {
        this.registrationService.Clear();
        this.exceptionHeaderService.nextException({
          messages: ['There is not invite link key url'],
          isReportButton: false
        });
        this.router.navigate(['/']);
        return false;
      }

    }

    this.storage.saveInRepository('linkKey', linkKey);

    const obs = this.registrationService.isWaitingConfirmation(linkKey).pipe(catchError(err => {
      this.registrationService.Clear();
      this.router.navigate(['/']);
      return throwError(err);
    }));

    obs.subscribe(value => {
      if (!value) {
        this.exceptionHeaderService.nextException({messages: ['Invite link key has been expired'], isReportButton: false});
        this.registrationService.Clear();
        this.router.navigate(['/']);
      }
    });

    return obs;
  }

}
